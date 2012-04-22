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


/*
 * 
 *  This element deals with loading an element and then rotates it.   
 * 
 */
namespace ToddlerTyper
{
    class DrawSpinAnimation : DrawAnimation
    {
        private int TURN_ANGLE = 5;
        private int speed = 1;
        private bool turnRight = true;

        private DrawElement sourceElement;
        List<Bitmap> elements = new List<Bitmap>();

        public DrawSpinAnimation(int posX, int posY, DrawElement element)
            : this(posX, posY, element, 1, true)
        {
        }

        public DrawSpinAnimation(int posX, int posY, DrawElement element, int speed, bool turnRight)
            : base(posX, posY)
        {
            this.sourceElement = element;
            this.speed = speed;
            this.turnRight = turnRight;

            cacheRotation();
        }

        /*
         * 
         *  Rotating an image in real time is fairly expensive.  We'll trade space for speed here with the caveat
         *  that we will not be making many of these in active use at a time.
         *
         */
        private void cacheRotation()
        {
            for (int i = 0; i < 360; i += TURN_ANGLE)
            {
                Bitmap image = new Bitmap(sourceElement.getWidth(), sourceElement.getHeight(), PixelFormat.Format32bppPArgb);
                Bitmap buffer = new Bitmap(sourceElement.getWidth(), sourceElement.getHeight(), PixelFormat.Format32bppPArgb);

                //make a graphics object from the empty bitmap
                Graphics g = Graphics.FromImage(image);
                sourceElement.draw(g);

                // rotate by TURN_ANGLE and cache it
                rotateBitmap(buffer, image, i);
                elements.Add(buffer);
            }
        }

        /*
         * 
         * Rotate bitmap by angle in degress
         * 
         */
        private void rotateBitmap(Bitmap bitmap, Bitmap source, float angle)
        {
            //make a graphics object from the empty bitmap
            Graphics g = Graphics.FromImage(bitmap);
            //move rotation point to center of image
            g.TranslateTransform((float)source.Width / 2, (float)source.Height / 2);
            //rotate
            g.RotateTransform(angle);
            //move image back
            g.TranslateTransform(-(float)source.Width / 2, -(float)source.Height / 2);
            //draw passed in image onto graphics object
            g.DrawImage(source, 0, 0, source.Width, source.Height);
        }

        public override int getHeight()
        {
            return sourceElement.getHeight();
        }

        public override int getWidth()
        {
            return sourceElement.getWidth();
        }

        /*
        * @param graphics  
        */
        public override void draw(Graphics graphics)
        {
            if (turnRight)
            {
                // this is just a sample draw routine.  It needs to be overrided by the child object
                Bitmap bitmap = elements[((int)getTicks() * speed % (360 / TURN_ANGLE))];
                graphics.DrawImageUnscaled(bitmap, getX(), getY(), bitmap.Width, bitmap.Height);
            }
            else
            {
                // this is just a sample draw routine.  It needs to be overrided by the child object
                Bitmap bitmap = elements[((360 / TURN_ANGLE) - ((int)getTicks() * speed % (360 / TURN_ANGLE)) - 1)];
                graphics.DrawImageUnscaled(bitmap, getX(), getY(), bitmap.Width, bitmap.Height);
            }
        }
    }
}
