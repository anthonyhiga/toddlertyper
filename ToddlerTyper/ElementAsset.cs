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
    class ElementAsset
    {
        private string sound;
        private string bitmap;

        

        public void addSoundFileName(string fileName)
        {
            sound = fileName;
        }

        public void addImageFileName(string fileName)
        {
            bitmap = fileName;
        }

        public void addFileName(string fileName, string type)
        {
            switch (type)
            {
                case "jpg":
                case "png":
                case "gif":
                case "bmp": 
                    addImageFileName(fileName);
                    break;
                case "wav":
                case "mp3":
                    addSoundFileName(fileName);
                    break;
            }
        }

        public string getSoundFileName()
        {
            return sound;
        }

        public string getImageFileName()
        {
            return bitmap;
        }
    }
}
