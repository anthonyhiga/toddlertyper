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
using System.Threading;

using System.Media;


/*
 * 
 *  Working with the player it become obvious playing the sound in sync caused for lots
 *  of rendering delays.  The Sound manager queues up sounds and plays them in the background
 *  away from the window's primary thread.
 * 
 */
namespace ToddlerTyper
{
    class SoundManager
    {
        private static LinkedList<SoundEvent> sounds = new LinkedList<SoundEvent>();
        private static List<Thread> threads = new List<Thread>();
        public SoundManager()
        {
            // Start up background threads
            for (int i = 0; i < 2; i++)
            {
                SoundThread soundThread = new SoundThread(this);
                Thread background = new Thread(new ThreadStart(soundThread.RunThread));
                Console.WriteLine("Starting audio background thread");
                threads.Add(background);
                background.Start();
            }
        }

        /*
         * 
         *   Allow for thread destruction
         * 
         */
        public void Shutdown()
        {
            foreach (Thread thread in threads)
            {
                thread.Abort();
            }
        }

        /*
         * 
         *   Queue up a sound to play
         * 
         */
        public void playSound(SoundEvent soundEvent)
        {
            lock (sounds)
            {
                sounds.AddFirst(soundEvent);
            }
        }

        /*
         * 
         *  Grabs then next sound to play from the queue
         * 
         */
        public SoundEvent getNextSoundEvent()
        {
            lock (sounds)
            {
                if (sounds.Count > 0)
                {
                    SoundEvent soundEvent = sounds.Last.Value;
                    sounds.RemoveLast();
                    return soundEvent;
                }
                else
                {
                    return null;
                }
            }
        }
    }

    /*
     * 
     *  Background helper thread definition
     * 
     */
    class SoundThread
    {
        SoundManager manager;

        public SoundThread(SoundManager manager)
        {
            this.manager = manager;
        }

        public void RunThread() 
        {
            while (true)
            {
                // check every 50 milliseconds... arbitrary, but seems ok.
                Thread.Sleep(50);
             
                // grab the next sound
                SoundEvent soundEvent = manager.getNextSoundEvent();
                if (soundEvent == null)
                    continue;

                Console.WriteLine("playing next sound");

                // play it!
                SoundPlayer player = new SoundPlayer(soundEvent.getFileName());
                player.Play();
            }
        }
    }
}
