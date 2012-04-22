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
 * Animation used to draw an element which looks like gravity has taken ahold of an object as it
 * bounces across the screen.
 * 
 * 
 * */
namespace ToddlerTyper
{
    class DrawBouncingAnimation : DrawAnimation
    {
        private int screenHeight;
        private DrawElement sourceElement;
        private int posX, posY, velocityX = 0, velocityY = 0;

        public DrawBouncingAnimation(int posX, int posY, int screenHeight, DrawElement element)
            : base(posX, posY)
        {
            this.sourceElement = element;
            this.screenHeight = screenHeight;
            this.posX = posX;
            this.posY = -element.getHeight();
            this.velocityX = posY % 30 - 15;
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
        */
        public override void draw(Graphics graphics)
        {
            if (posY > screenHeight - sourceElement.getHeight())
            {
                velocityY = -(int)(velocityY * 0.8);
                posY = screenHeight - sourceElement.getHeight();
            }
            else
            {
                velocityY++;
                posY += velocityY;
                posX += velocityX;
            }

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
