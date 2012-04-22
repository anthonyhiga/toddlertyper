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
 *  This element takes care of loading and caching an Movie file into memory. 
 * 
 */
namespace ToddlerTyper
{
    class DrawMovieElement : DrawElement
    {
        private AxWMPLib.AxWindowsMediaPlayer axWindowsMediaPlayer1;

        public DrawMovieElement(string fileName) : this(0, 0, fileName)
        {
        }

        public DrawMovieElement(int posX, int posY, string fileName) : base(posX, posY)
        {
            this.fileName = fileName;

            this.axWindowsMediaPlayer1 = new AxWMPLib.AxWindowsMediaPlayer();
            ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).BeginInit();

            // 
            // axWindowsMediaPlayer1
            // 
            this.axWindowsMediaPlayer1.Enabled = true;
            this.axWindowsMediaPlayer1.Location = new System.Drawing.Point(422, 304);
            this.axWindowsMediaPlayer1.Name = "axWindowsMediaPlayer1";
            this.axWindowsMediaPlayer1.Size = new System.Drawing.Size(75, 23);
            this.axWindowsMediaPlayer1.TabIndex = 0;
            this.axWindowsMediaPlayer1.URL = "C:\\Documents and Settings\\Administrator\\My Documents\\Mass Effect 3 - Synthesis Ending(SPOILERS)!.wmv";
        }


        private string fileName;

        public override int getHeight()
        {
            return 0;
        }

        public override int getWidth()
        {
            return 0;
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
            //graphics.DrawMovieUnscaled(Movie, getX(), getY(), Movie.Width, Movie.Height);
        }
    }
}
