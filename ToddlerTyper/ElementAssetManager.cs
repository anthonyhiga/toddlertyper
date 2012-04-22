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
using System.Text.RegularExpressions;
using System.IO;


/*
 * 
 *  Element Asset Manager, is responsible for abstracting out where the element data comes from.
 *  We could even potentially load data from the web instead, allowing someone to load images from 
 *  Facebook or other sources.   
 * 
 */
namespace ToddlerTyper
{
    class ElementAssetManager
    {
        public ElementAssetManager()
        {
            LoadElementAssetsFromDisk();
        }

        /*
         * 
         *   We'll use the file system and a bunch of naming convetions and directories
         *   to decide what belongs to an element.
         * 
         *   This is a really short term approach.  Ideally using a manifest file per element
         *   or even a directory per element would be better, allowing a more flexible method
         *   of defining elements
         * 
         */
        private void LoadElementAssetsFromDisk()
        {
            string filePattern = "(?<entity>default|element)_(?<name>.+)\\.(?<type>jpg|png|gif|bmp|wav|mp3)";
            Regex rx = new Regex(filePattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

            string elementPath = Directory.GetCurrentDirectory() + "\\Elements";

            foreach (string fileName in Directory.GetFiles(elementPath))
            {
                // Find matches.
                MatchCollection matches = rx.Matches(fileName);

                // Report the number of matches found.
                Console.WriteLine("{0} matches found.", matches.Count);

                // Report on each match.
                foreach (Match match in matches)
                {
                    string entity = match.Groups["entity"].Value;
                    string name = match.Groups["name"].Value;
                    string type = match.Groups["type"].Value;

                    if (entity == "default")
                    {
                        defaults.Add(fileName);
                    }
                    else
                    {
                        ElementAsset asset;
                        if (!elements.ContainsKey(name))
                        {
                            asset = new ElementAsset();
                            elements.Add(name, asset);
                        }
                        else
                        {
                            asset = elements[name];
                        }

                        asset.addFileName(fileName, type);
                    }
                }

                Console.WriteLine(fileName);
            }

            assets.AddRange(elements.Values);

            // inject default sound for every element which is missing a sound
            foreach (ElementAsset asset in assets)
            {
                if (asset.getSoundFileName() == null && defaults.Count > 0)
                {
                    asset.addSoundFileName(defaults[random.Next(defaults.Count)]);
                }
            }
        }


        private Dictionary<string, ElementAsset> elements = new Dictionary<string, ElementAsset>();
        private List<ElementAsset> assets = new List<ElementAsset>();
        private List<string> defaults = new List<string>();


        /*
         * 
         *  return a random element
         * 
         */
        private Random random = new Random();
        public ElementAsset getRandomElementAsset()
        {
            int index = random.Next(assets.Count);
            return assets[index];
        }

        /**
         * 
         *   Return an element based on the key code pressed. 
         *
         */
        public ElementAsset getElementAsset(int key)
        {
            if (elements.ContainsKey(key.ToString()))
            {
                return elements[key.ToString()];
            }
            else
            {
                return getRandomElementAsset();
            }
        }
    }
}
