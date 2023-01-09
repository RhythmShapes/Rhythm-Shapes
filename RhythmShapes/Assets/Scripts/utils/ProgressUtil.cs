using System;

namespace utils
{
    public static class ProgressUtil
    {
        public static float Progress { get; private set; }
        public static float Total { get; private set; }

        public static void Init(int total)
        {
            SetTotal(total);
            Reset();
        }
        
        public static void Init(float total)
        {
            SetTotal(total);
            Reset();
        }

        public static void SetTotal(int total)
        {
            if (total <= 0)
                throw new Exception("Total cannot be <= 0");
            
            Total = total;
        }

        public static void SetTotal(float total)
        {
            if (total <= 0)
                throw new Exception("Total cannot be <= 0");
            
            Total = total;
        }

        public static void Reset()
        {
            Progress = 0f;
        }

        public static void Update()
        {
            Progress++;
        }

        public static void Update(int amount)
        {
            Progress += amount;
        }

        public static void Update(float amount)
        {
            Progress += amount;
        }

        public static float ToPercent()
        {
            return Progress / Total;
        }

        public static bool IsComplete()
        {
            return Math.Abs(Progress - Total) >= 0;
        }
    }
}