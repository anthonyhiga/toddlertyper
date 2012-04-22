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
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;


/**
 * 
 * Element Manager Singleton, meant to manage creation and storage of elements in memory
 * It is multi-threaded to allow elements to be created in the background independent of the  
 * windowed drawing routines.
 * 
 * Elements are Queued up via a lightweight syncronization routine and then created and injected into
 * the manager's global element list.
 * 
 * */
namespace ToddlerTyper
{
    class DrawElementManager
    {
        private int MAX_ELEMENTS = 20;
        private int BACKGROUND_THREADS = 2;

        public DrawElementManager()
        {
            InitElementGenerators();
        }


        /*****************************************************************************************/
        /**
         *    Allow the window manager to let the element manager know if the screen size has changed.
         * */
        private int screenWidth, screenHeight;
        public void setScreenWidth(int screenWidth)
        {
            this.screenWidth = screenWidth;
        }

        public void setScreenHeight(int screenHeight)
        {
            this.screenHeight = screenHeight;
        }


        /*****************************************************************************************/
        // public methods
        public LinkedList<DrawElement> getElements()
        {
            return elements;
        }

        public void addRandomDrawElement(DrawElementRequest request)
        {
            lock (requests)
            {
                requests.AddLast(request);
            }
        }

        public void createNextDrawElement()
        {
            DrawElementRequest request;
            lock (requests)
            {
                if (requests.Count == 0)
                    return;
                request = requests.First.Value;
                requests.RemoveFirst();
            }

            // chose an animation engine
            int type = random.Next(animatedElementGenerators.Count);
            DrawElement element = animatedElementGenerators[type](request);

            // maintain size of ELEMENTS
            lock (elements)
            {
                // Remove element from list if there are more than the max.
                if (elements.Count > MAX_ELEMENTS)
                    elements.RemoveFirst();

                if (element != null)
                    elements.AddLast(element);
            }
        }

        /**
         * Kill the background threads
         * */
        public void Shutdown()
        {
            foreach (Thread thread in threads)
            {
                thread.Abort();
            }
        }

        public void nextFrame()
        {
            // increment the animation of each element
            List<DrawElement> toRemove = new List<DrawElement>();
            List<DrawElement> copyElements;

            lock (elements)
            {
                copyElements = new List<DrawElement>(elements);
            }

            foreach (DrawElement element in copyElements)
            {
                if (element.isDone())
                {
                    toRemove.Add(element);
                }
                else
                {
                    element.tick();
                }
            }

            lock (elements)
            {
                // remove from the elements list the elements which have expired.
                foreach (DrawElement element in toRemove)
                {
                    elements.Remove(element);
                }
            }
        }



        private delegate DrawElement CreateElement(DrawElementRequest request);
        private List<CreateElement> animatedElementGenerators = new List<CreateElement>();
        private List<CreateElement> staticElementGenerators = new List<CreateElement>();

        private static List<Thread> threads = new List<Thread>();

        private LinkedList<DrawElement> elements = new LinkedList<DrawElement>();
        private static LinkedList<DrawElementRequest> requests = new LinkedList<DrawElementRequest>();

        private Random random = new Random();

        //*****************************************************************************************/
        //  Element Generators


        /**
         *   Generator initialization.
         * 
         */
        private void InitElementGenerators()
        {
            // these are used to generate unmoving elements
            StaticWeightedGenerator(this.CreateDrawStaticImage, 37);
            StaticWeightedGenerator(this.CreateDrawPulsingImage, 17);
            StaticWeightedGenerator(this.CreateDrawSpinAnimationRightSpeedLow, 1);
            StaticWeightedGenerator(this.CreateDrawSpinAnimationRightSpeedMid, 1);
            StaticWeightedGenerator(this.CreateDrawSpinAnimationRightSpeedHigh, 1);
            StaticWeightedGenerator(this.CreateDrawSpinAnimationLeftSpeedLow, 1);
            StaticWeightedGenerator(this.CreateDrawSpinAnimationLeftSpeedMid, 1);
            StaticWeightedGenerator(this.CreateDrawSpinAnimationLeftSpeedHigh, 1);

            //// these are used to generate moving elements
            AnimatedWeightedGenerator(this.CreateDrawStaticImage, 3);
            AnimatedWeightedGenerator(this.CreateDrawPulsingImage, 3);
            AnimatedWeightedGenerator(this.CreateDrawFallingAnimationSpeedLow, 6);
            AnimatedWeightedGenerator(this.CreateDrawFallingAnimationSpeedMid, 6);
            AnimatedWeightedGenerator(this.CreateDrawFallingAnimationSpeedHigh, 6);
            AnimatedWeightedGenerator(this.CreateDrawFloatingAnimation, 9);
            AnimatedWeightedGenerator(this.CreateDrawBouncingAnimation, 9);
            AnimatedWeightedGenerator(this.CreateDrawBorderBounceAnimation, 9);
            AnimatedWeightedGenerator(this.CreateDrawSpiralAnimation, 9);
            AnimatedWeightedGenerator(this.CreateDrawTeleportAnimation, 9);

            // setup background threads which create elements
            for (int i = 0; i < BACKGROUND_THREADS; i++)
            {
                DrawElementThread drawElementThread = new DrawElementThread(this);
                Thread background = new Thread(new ThreadStart(drawElementThread.RunThread));
                Console.WriteLine("Starting draw element background thread");
                threads.Add(background);
                background.Start();
            }
        }

        private void AnimatedWeightedGenerator(CreateElement createElement, int weight)
        {
            for (int i = 0; i < weight; i++)
            {
                animatedElementGenerators.Add(createElement);
            }
        }

        private void StaticWeightedGenerator(CreateElement createElement, int weight)
        {
            for (int i = 0; i < weight; i++)
            {
                staticElementGenerators.Add(createElement);
            }
        }

        private DrawElement CreateDrawImage(DrawElementRequest request)
        {
            return new DrawImageElement(request.getAsset().getImageFileName());
        }

        private DrawElement CreateDrawStaticElement(DrawElementRequest request)
        {
            int type = random.Next(staticElementGenerators.Count);
            DrawElement element = staticElementGenerators[type](request);

            // normalize to zero
            element.setX(0);
            element.setY(0);
            return element;
        }

        private DrawElement CreateDrawStaticImage(DrawElementRequest request)
        {
            DrawElement element = CreateDrawImage(request);
            element.setX(request.getCalculatedX(element.getWidth()));
            element.setY(request.getCalculatedY(element.getHeight()));
            return element;
        }

        private DrawElement CreateDrawPulsingImage(DrawElementRequest request)
        {
            DrawElement element = new DrawPulsingImageElement(request.getAsset().getImageFileName());
            element.setX(request.getCalculatedX(element.getWidth()));
            element.setY(request.getCalculatedY(element.getHeight()));
            return element;
        }

        private DrawElement CreateDrawSpinAnimationRightSpeedLow(DrawElementRequest request)
        {
            DrawElement element = CreateDrawStaticElement(request);
            return new DrawSpinAnimation(request.getCalculatedX(element.getWidth()), request.getCalculatedY(element.getHeight()), element, 1, true);
        }

        private DrawElement CreateDrawSpinAnimationRightSpeedMid(DrawElementRequest request)
        {
            DrawElement element = CreateDrawImage(request);
            return new DrawSpinAnimation(request.getCalculatedX(element.getWidth()), request.getCalculatedY(element.getHeight()), element, 2, true);
        }

        private DrawElement CreateDrawSpinAnimationRightSpeedHigh(DrawElementRequest request)
        {
            DrawElement element = CreateDrawImage(request);
            return new DrawSpinAnimation(request.getCalculatedX(element.getWidth()), request.getCalculatedY(element.getHeight()), element, 3, true);
        }

        private DrawElement CreateDrawSpinAnimationLeftSpeedLow(DrawElementRequest request)
        {
            DrawElement element = CreateDrawImage(request);
            return new DrawSpinAnimation(request.getCalculatedX(element.getWidth()), request.getCalculatedY(element.getHeight()), element, 1, false);
        }

        private DrawElement CreateDrawSpinAnimationLeftSpeedMid(DrawElementRequest request)
        {
            DrawElement element = CreateDrawImage(request);
            return new DrawSpinAnimation(request.getCalculatedX(element.getWidth()), request.getCalculatedY(element.getHeight()), element, 2, false);
        }

        private DrawElement CreateDrawSpinAnimationLeftSpeedHigh(DrawElementRequest request)
        {
            DrawElement element = CreateDrawImage(request);
            return new DrawSpinAnimation(request.getCalculatedX(element.getWidth()), request.getCalculatedY(element.getHeight()), element, 3, false);
        }

        private DrawElement CreateDrawFallingAnimationSpeedLow(DrawElementRequest request)
        {
            DrawElement element = CreateDrawStaticElement(request);
            return new DrawFallingAnimation(request.getCalculatedX(element.getWidth()), request.getCalculatedY(element.getHeight()), screenHeight, 60, element);
        }

        private DrawElement CreateDrawFallingAnimationSpeedMid(DrawElementRequest request)
        {
            DrawElement element = CreateDrawStaticElement(request);
            return new DrawFallingAnimation(request.getCalculatedX(element.getWidth()), request.getCalculatedY(element.getHeight()), screenHeight, 120, element);
        }

        private DrawElement CreateDrawFallingAnimationSpeedHigh(DrawElementRequest request)
        {
            DrawElement element = CreateDrawStaticElement(request);
            return new DrawFallingAnimation(request.getCalculatedX(element.getWidth()), request.getCalculatedY(element.getHeight()), screenHeight, 180, element);
        }

        private DrawElement CreateDrawFloatingAnimation(DrawElementRequest request)
        {
            DrawElement element = CreateDrawStaticElement(request);
            return new DrawFloatingAnimation(request.getCalculatedX(element.getWidth()), request.getCalculatedY(element.getHeight()), screenWidth, screenHeight, element);
        }

        private DrawElement CreateDrawBouncingAnimation(DrawElementRequest request)
        {
            DrawElement element = CreateDrawStaticElement(request);
            return new DrawBouncingAnimation(request.getCalculatedX(element.getWidth()), request.getCalculatedY(element.getHeight()), screenHeight, element);
        }

        private DrawElement CreateDrawBorderBounceAnimation(DrawElementRequest request)
        {
            DrawElement element = CreateDrawStaticElement(request);
            return new DrawBorderBounceAnimation(request.getCalculatedX(element.getWidth()), request.getCalculatedY(element.getHeight()), screenWidth, screenHeight, element);
        }

        private DrawElement CreateDrawSpiralAnimation(DrawElementRequest request)
        {
            DrawElement element = CreateDrawStaticElement(request);
            return new DrawSpiralAnimation(request.getCalculatedX(element.getWidth()), request.getCalculatedY(element.getHeight()), screenWidth, screenHeight, element);
        }

        private DrawElement CreateDrawTeleportAnimation(DrawElementRequest request)
        {
            DrawElement element = CreateDrawStaticElement(request);
            return new DrawTeleportAnimation(request.getCalculatedX(element.getWidth()), request.getCalculatedY(element.getHeight()), screenWidth, screenHeight, element);
        }
    }


    /**
     *    Draw Element thread which allows the manager to create elements in the background
     * 
     * */
    class DrawElementThread
    {
        DrawElementManager manager;

        public DrawElementThread(DrawElementManager manager)
        {
            this.manager = manager;
        }

        public void RunThread()
        {
            // looped thread which looks every 50 milliseconds
            while (true)
            {
                Thread.Sleep(50);
                manager.createNextDrawElement();
            }
        }
    }
}
