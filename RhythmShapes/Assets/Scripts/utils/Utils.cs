using System;

namespace utils
{
    public static class Utils
    {
        private const int TimeRound = 3;

        public static float RoundTime(float time)
        {
            return (float) Math.Round(time, TimeRound);
        }
    }
}