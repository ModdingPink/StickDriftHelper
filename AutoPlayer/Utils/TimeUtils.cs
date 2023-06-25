using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPlayer.Utils
{
    internal class TimeUtils
    {

        public static float SecondsToBeats(float bpm, float seconds)
        {
            return seconds * (bpm / 60.0f);
        }

        public static float BeatsToSeconds(float bpm, float beat)
        {
            return (60.0f / bpm) * beat;
        }

    }
}
