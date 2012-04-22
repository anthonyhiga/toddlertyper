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
 *   Animation Element which makes an element look like it's floating
 *
 */
namespace ToddlerTyper
{
    class DrawSpiralAnimation : DrawAnimation
    {
        private int screenWidth, screenHeight, posX, posY;
        private DrawElement sourceElement;

        public DrawSpiralAnimation(int posX, int posY, int screenWidth, int screenHeight, DrawElement element)
            : base(posX, posY)
        {
            this.sourceElement = element;
            this.screenHeight = screenHeight;
            this.screenWidth = screenWidth;
            this.posX = posX;
            this.posY = posY;
        }

        public override int getHeight()
        {
            return sourceElement.getHeight();
        }

        public override int getWidth()
        {
            return sourceElement.getWidth();
        }

        public override void tick()
        {
            sourceElement.tick();
            base.tick();
        }

        /*
        * @param graphics  
        * @param timeMS
        */


        public override void draw(Graphics graphics)
        {
            // this is just a sample draw routine.  It needs to be overrided by the child object
            double width = getTicks() * 1.5;
            sourceElement.setX(posX + (int)(Math.Sin(getTicks() % 360 * 0.20) * width));
            sourceElement.setY(posY + (int)(Math.Cos(getTicks() % 360 * 0.20) * width));
            sourceElement.draw(graphics);
        }
    }
}
