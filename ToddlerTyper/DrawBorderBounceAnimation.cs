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


/**
 * 
 *   Animation Element which moves an element around given a random starting velocity and allows the element to 
 *   hit a virtual border and bounce around.
 *
 */
namespace ToddlerTyper
{
    class DrawBorderBounceAnimation : DrawAnimation
    {
        // Private vars
        private int screenHeight, screenWidth;
        private DrawElement sourceElement;
        private int posX, posY, velocityX, velocityY;
        private Random random = new Random();

        public DrawBorderBounceAnimation(int posX, int posY, int screenWidth, int screenHeight, DrawElement element)
            : base(posX, posY)
        {
            this.sourceElement = element;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            this.posX = posX;
            this.posY = posY;
            velocityX = random.Next(40) - 20;
            velocityY = random.Next(40) - 20;
        }

        public override void tick()
        {
            sourceElement.tick();
            base.tick();
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
        * @param timeMS
        */
        public override void draw(Graphics graphics)
        {
            // move the element a little bit based on X axis
            if (posX > screenWidth - sourceElement.getWidth())
            {
                velocityX = -velocityX;
                posX = screenWidth - sourceElement.getWidth();
            }
            else if (posX < 0)
            {
                velocityX = -velocityX;
                posX = 0;
            }
            else
            {
                posX += velocityX;
            }

            // move the element a little bit based on the Y axis
            if (posY > screenHeight - sourceElement.getHeight())
            {
                velocityY = -velocityY;
                posY = screenHeight - sourceElement.getHeight();
            }
            else if (posY < 0)
            {
                velocityY = -velocityY;
                posY = 0;
            }
            else
            {
                posY += velocityY;
            }

            // draw the element
            sourceElement.setX(posX);
            sourceElement.setY(posY);
            sourceElement.draw(graphics);
        }

        /**
         *   To be overriden.   Allows element to communicate it should be deleted.
         * */
        public override bool isDone()
        {
            if (getTicks() > 280)
                return true;
            return false;
        }
    }
}
