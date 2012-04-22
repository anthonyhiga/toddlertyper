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
using System.Drawing;
using System.Drawing.Imaging;

namespace ToddlerTyper
{
    // NOTE: there is an assumption here that elements will never interact with each other.  Because of this
    // we'll allow positional data to be tracked here.  However if there is potential for collisions
    // then quite possubly a more "global" method of collision detection may be necessary
    class DrawElement
    {
        private int posX, posY;
        private long ticks;

        public DrawElement(int posX, int posY)
        {
            this.posX = posX;
            this.posY = posY;
        }

        /**
         *  used to increment animation of this element.
         * */
        public virtual void tick()
        {
            ticks++;
        }

        /*
         *  returns a hash key which allows other nodes to compare this element with another
         *  allows you to determine if one node is logically the same as another.
         */
        public virtual string getHashKey()
        {
            return "";
        }

        public void setX(int value)
        {
            posX = value;
        }

        public void setY(int value)
        {
            posY = value;
        }

        public long getTicks()
        {
            return ticks;
        }

        public int getX()
        {
            return posX;
        }

        public int getY()
        {
            return posY;
        }

        public virtual int getWidth()
        {
            return 100;
        }

        public virtual int getHeight()
        {
            return 100;
        }

        /*
         * To be overridden, Base element designed to 
         * 
         * @param graphics  
         */
        public virtual void draw(Graphics graphics)
        {
            // this is just a sample draw routine.  It needs to be overrided by the child object it draws a pretty red line... useless
            //  unless you like red lines.
            Pen myPen = new Pen(System.Drawing.Color.Red, 5);
            graphics.DrawLine(myPen, posX, posY, posX + 100, posY + 100);
        }

        /**
         *   To be overriden.   Allows element to communicate it should be deleted.
         * */
        public virtual bool isDone() 
        {
            if (ticks > 200) // default only live for 100 ticks
                return true;
            return false;
        }


        /*
         * copies data from image to bitmap converting format
         */
        protected Bitmap copyImageToBitmap(Image b)
        {
            int border = 50;
            int side = b.Width > b.Height ? b.Width : b.Height;
            Bitmap result = new Bitmap(side + border * 2, side + border * 2, PixelFormat.Format32bppPArgb);

            //make a graphics object from the empty bitmap
            Graphics g = Graphics.FromImage(result);

            g.DrawImage(b, (side - b.Width) / 2 + border, (side - b.Height) / 2 + border, b.Width, b.Height);
            return result;
        }
    }
}
