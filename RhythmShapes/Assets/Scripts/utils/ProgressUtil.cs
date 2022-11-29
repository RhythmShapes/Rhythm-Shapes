using System;

namespace utils
{
    public class ProgressUtil
    {
        public float Progress { get; private set; }
        public float Total { get; private set; }

        public void Init(int total)
        {
            SetTotal(total);
            Reset();
        }
        
        public void Init(float total)
        {
            SetTotal(total);
            Reset();
        }

        public void SetTotal(int total)
        {
            if (total <= 0)
                throw new Exception("Total cannot be <= 0");
            
            Total = total;
        }

        public void SetTotal(float total)
        {
            if (total <= 0)
                throw new Exception("Total cannot be <= 0");
            
            Total = total;
        }

        public void Reset()
        {
            Progress = 0f;
        }

        public void Update()
        {
            Progress++;
        }

        public void Update(int amount)
        {
            Progress += amount;
        }

        public void Update(float amount)
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