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

/**
 *  Simple Parent class for all element nodes which do some form of animation.  Please note for animation purposes, the posX and posY are for
 *    suggestion only!  The actual element class as the option to override and use the values at their discretion.
 */
namespace ToddlerTyper
{
    class DrawAnimation : DrawElement
    {
        public DrawAnimation(int posX, int posY) : base(posX, posY)
        {
        }
    }
}
