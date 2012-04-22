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


/*
 *  Simple model object for representing the assets associated with a given element.
 *  It is abstract from the final element rendering and "could" be made to 
 *  allow for options such as multiple input element images or sounds.
 */
namespace ToddlerTyper
{
    class DrawElementRequest
    {
        private ElementAsset asset;

        private int screenWidth, screenHeight, posX, posY;

        public DrawElementRequest(ElementAsset asset, int posX, int posY, int screenHeight, int screenWidth)
        {
            this.asset = asset;
            this.posY = posY;
            this.posX = posX;
            this.screenHeight = screenHeight;
            this.screenWidth = screenWidth;
        }
        
        public DrawElementRequest(ElementAsset asset, int screenHeight, int screenWidth) 
            : this(asset, -1, -1, screenWidth, screenWidth)
        {
            
        }

        Random random = new Random();
        public int getCalculatedX(int width)
        {
            if (posX < 0)
            {
                return random.Next(screenWidth - width);
            }

            return posX - width / 2;
        }

        public int getCalculatedY(int height)
        {
            if (posY < 0)
            {
                return posY = random.Next(screenHeight - height);
            }

            return posY - height / 2;
        }

        public int getScreenWidth()
        {
            return screenWidth;
        }

        public int getScreenHeight()
        {
            return screenHeight;
        }

        public int getX()
        {
            return posX;
        }

        public int getY()
        {
            return posY;
        }

        public ElementAsset getAsset()
        {
            return asset;
        }
    }
}
