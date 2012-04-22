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
 *   Animation Element which drops an element from top to bottom of the screen.
 *
 */
namespace ToddlerTyper
{
    class DrawFallingAnimation : DrawAnimation
    {
        private int duration;
        private int screenHeight;
        private DrawElement sourceElement;

        public DrawFallingAnimation(int posX, int posY, int screenHeight, int duration, DrawElement element)
            : base(posX, posY)
        {
            this.duration = duration;
            this.sourceElement = element;
            this.screenHeight = screenHeight;
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
            int drop = screenHeight + sourceElement.getHeight() * 2;
            long posY = (drop * getTicks()) / duration - sourceElement.getHeight();

            sourceElement.setX(getX());
            sourceElement.setY(getY());
            sourceElement.draw(graphics);
        }

        /**
         *   To be overriden.   Allows element to communicate it should be deleted.
         * */
        public override bool isDone()
        {
            if (getTicks() > duration)
                return true;
            return false;
        }
    }
}
