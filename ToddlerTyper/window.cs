/**
   Copyright 2012 Useless Random Thought Software

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 **/


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Timers;
using System.Media;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace ToddlerTyper
{
    public partial class MainWindow : Form
    {
        //*****************************************************************************************/
        // setting up unmanaged code path

        // Structure contain information about low-level keyboard input event
        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public Keys key;
            public int scanCode;
            public int flags;
            public int time;
            public IntPtr extra;
        }

        //System level functions to be used for hook and unhook keyboard input
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int id, LowLevelKeyboardProc callback, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hook);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hook, int nCode, IntPtr wp, IntPtr lp);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string name);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern short GetAsyncKeyState(Keys key);

        //Declaring Global objects
        private IntPtr ptrHook;
        private LowLevelKeyboardProc objKeyboardProcess;

        /*
         *  Setting up special key interceptor 
         */
        private IntPtr captureKey(int nCode, IntPtr wp, IntPtr lp)
        {
            if (nCode >= 0)
            {
                KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lp, typeof(KBDLLHOOKSTRUCT));
                if (objKeyInfo.key == Keys.RWin || objKeyInfo.key == Keys.LWin) // Disabling Windows keys
                {
                    return (IntPtr)1;
                }
            }
            return CallNextHookEx(ptrHook, nCode, wp, lp);
        }


        //*****************************************************************************************/
        /**
         * Constructor
         * */
        public MainWindow()
        {
            ProcessModule objCurrentModule = Process.GetCurrentProcess().MainModule; //Get Current Module
            objKeyboardProcess = new LowLevelKeyboardProc(captureKey); //Assign callback function each time keyboard process
            ptrHook = SetWindowsHookEx(13, objKeyboardProcess, GetModuleHandle(objCurrentModule.ModuleName), 0); //Setting Hook of Keyboard Process for current module

            InitializeComponent();
            Init();
        }


        //*****************************************************************************************/
        // manager singleton objects
        private ElementAssetManager elementAssetManager = new ElementAssetManager();
        private SoundManager soundManager = new SoundManager();
        private DrawElementManager drawElementManager = new DrawElementManager();

        //*****************************************************************************************/


        private void Init()
        {
            // setup timer
            frameRefreshTimer = new System.Timers.Timer();
            frameRefreshTimer.Elapsed += new ElapsedEventHandler(OnRefreshEvent);
            frameRefreshTimer.Interval = 68;
            frameRefreshTimer.Enabled = true;
            frameRefreshTimer.Start();

            frameRateTimer = new System.Timers.Timer();
            frameRateTimer.Elapsed += new ElapsedEventHandler(OnFrameRateCheck);
            frameRateTimer.Interval = 2000;
            frameRateTimer.Enabled = true;
            frameRateTimer.Start();

        }

        //*****************************************************************************************/
        private void OnFrameRateCheck(object sender, ElapsedEventArgs e)
        {
            int elementCount = 0;
            LinkedList<DrawElement> elements = drawElementManager.getElements();
            lock (elements)
            {
                elementCount = elements.Count;
            }

            GC.Collect();
            frameRate = frameCount / 2;
            frameCount = 0;
        }

        private void OnRefreshEvent(object sender, ElapsedEventArgs e)
        {
            // The force the window to redraw
            drawElementManager.nextFrame();

            Invalidate();
        }
        //*****************************************************************************************/

        private System.Timers.Timer frameRefreshTimer;
        private System.Timers.Timer frameRateTimer;


        private int lastKey = 0;
        private int frameCount = 0;
        private float frameRate = 0;
        private long frameTotal = 0;

        //*****************************************************************************************/
        //  System Events
        private void MainWindow_Paint(object sender, PaintEventArgs e)
        {

            // wipe the screen
            e.Graphics.Clear(Color.White);

            // draw the instructions
            Font drawFont = new Font("Arial", 12);
            SolidBrush drawBrush = new SolidBrush(Color.Black);
            // Set format of string.
            StringFormat drawFormat = new StringFormat();
            drawFormat.FormatFlags = StringFormatFlags.DirectionVertical;

            // draw string
            e.Graphics.DrawString(Properties.Resources.instructions, drawFont, drawBrush, 0, 0, drawFormat);

            if (frameTotal < 20 || (frameTotal < 600 && lastKey == 0))
            {
                e.Graphics.DrawString(Properties.Resources.copyright, drawFont, drawBrush, 100, 200);
            }

            // paint each element           
            LinkedList<DrawElement> elements = drawElementManager.getElements();
            lock (elements)
            {
                foreach (DrawElement element in elements)
                {
                    element.draw(e.Graphics);
                }
            }

            // display debugging info
            frameCount++;
            frameTotal++;

//#if DEBUG
//            string drawString = "Element Count: " + elements.Count + " Frame Rate: " + frameRate + " Last Key: " + lastKey;

//            // Draw string to screen.
//            e.Graphics.DrawString(drawString, drawFont, drawBrush, 30, 0, drawFormat);
//#endif
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            Cursor.Hide();
            drawElementManager.setScreenHeight(this.Height);
            drawElementManager.setScreenWidth(this.Width);
        }

        private void MainWindow_ResizeEnd(object sender, EventArgs e)
        {
            drawElementManager.setScreenHeight(this.Height);
            drawElementManager.setScreenWidth(this.Width);
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            soundManager.Shutdown();
            drawElementManager.Shutdown();
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            //NOTE: overriding to prevent tearing and other nasty graphical stuffs... 
            // and yes... we really are doing NOTHING here.
        }

        /*
         * 
         *  We only want one event per key press. We dedupe the "down" events
         *  by only counting them once.
         * 
         */
        private bool keyDown = false; 
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (keyDown)
                return;

            lastKey = e.KeyValue;
            RunEvent(e.KeyValue);
            keyDown = true;
        }

        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            keyDown = false;
        }

        private void MainWindow_Leave(object sender, EventArgs e)
        {
            // Get the window to the front.
            this.TopMost = true;
            this.TopMost = false;

            // 'Steal' the focus.
            this.Activate();
        }

        private void MainWindow_MouseDown(object sender, MouseEventArgs e)
        {
            RunEvent(0, e.X, e.Y);
        }


        //*****************************************************************************************/
        //  Element Randomizer
        private void RunEvent(int key)
        {
            RunEvent(key, -1, -1);
        }

        private void RunEvent(int key, int posX, int posY)
        {
            // notify the element manager of current screen size
            drawElementManager.setScreenHeight(this.Height);
            drawElementManager.setScreenWidth(this.Width);

            // grab an element asset
            ElementAsset asset = elementAssetManager.getElementAsset(key);
            if (asset.getImageFileName() == null)
            {
                // elements with no images are useless
                return;
            }

            DrawElementRequest request = new DrawElementRequest(asset, posX, posY, this.Height, this.Width);
            drawElementManager.addRandomDrawElement(request);

            // play sound
            if (asset.getSoundFileName() != null)
            {
                soundManager.playSound(new SoundEvent(asset.getSoundFileName()));
            }
        }
    }
}