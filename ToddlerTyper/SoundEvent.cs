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
 * 
 *  Simple model object for playing sound effects.  Used in conjunction with the manager.
 * * 
 */
namespace ToddlerTyper
{
    class SoundEvent
    {
        string fileName;

        public SoundEvent(string fileName)
        {
            this.fileName = fileName;
        }

        public string getFileName()
        {
            return fileName;
        }
    }
}
