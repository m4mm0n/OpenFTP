using System;

namespace Explorator
{
    public class ProgressExEventArgs : EventArgs
    {
        public long MaximumValue { get; set; }
        public long CurrentValue { get; set; }
        public string CurrentSpeed { get; set; }

        public float CurrentPercentage => ((float) CurrentValue / (float) MaximumValue) * 100f;

        public ProgressExEventArgs(long curValue, long maxValue, string curSpeed)
        {
            MaximumValue = maxValue;
            CurrentValue = curValue;
            CurrentSpeed = curSpeed;
        }

    }
}
