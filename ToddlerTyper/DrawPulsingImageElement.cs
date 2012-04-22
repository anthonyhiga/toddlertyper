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
 *  This element takes care of drawing a pulsing version of an image. By applying a simple sine function.
 *
 */
namespace ToddlerTyper
{
    class DrawPulsingImageElement : DrawElement
    {
        public DrawPulsingImageElement(string fileName) : this(0, 0, fileName)
        {
        }

        public DrawPulsingImageElement(int posX, int posY, string fileName) : base(posX, posY)
        {
            this.fileName = fileName;
            
            Image inputImage = Image.FromFile(fileName);
            image = copyImageToBitmap(inputImage);
        }


        private string fileName;
        private Bitmap image;


        public override int getHeight()
        {
            return image.Height;
        }

        public override int getWidth()
        {
            return image.Width;
        }

        public override string getHashKey()
        {
            return fileName;
        }


        /*
         * @param graphics  
         * @param timeMS
         */
        public override void draw(Graphics graphics)
        {
            // this is just a sample draw routine.  It needs to be overrided by the child object
            int t = (int)getTicks();
            int pixelsToGrowBy = (int) (Math.Sin(t * 0.8) * 20);
            graphics.DrawImage(image, getX() - pixelsToGrowBy, getY() - pixelsToGrowBy, image.Width + pixelsToGrowBy * 2, image.Height + pixelsToGrowBy * 2);
        }
    }
}
