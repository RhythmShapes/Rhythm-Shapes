using System;

namespace utils
{
    public class ProgressUtil
    {
        public float Progress { get; private set; }
        public float Total { get; private set; }

        public void Init(float total)
        {
            if (total <= 0)
                throw new Exception("Total cannot be <= 0");
            
            Progress = 0f;
            Total = total;
        }

        public void TaskDone()
        {
            Progress++;
        }

        public void TaskDone(float amount)
        {
            Progress += amount;
        }

        public float ToPercent()
        {
            return Progress / Total;
        }

        public bool IsComplete()
        {
            return Math.Abs(Progress - Total) >= 0;
        }
    }
}