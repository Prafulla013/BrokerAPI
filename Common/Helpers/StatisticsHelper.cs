using System;

namespace Common.Helpers
{
    public static class StatisticsHelper
    {
        public static int Median(this int[] array)
        {
            Array.Sort(array);

            double midIndex;
            if (array.Length % 2 == 0)
            {
                midIndex = array.Length / 2.0;
                // Since array index start with 0
                return (array[(int)midIndex - 1] + array[(int)(midIndex)]) / 2;
            }

            midIndex = (array.Length + 1) / 2;
            // Since array index start with 0
            return array[(int)midIndex - 1];
        }
    }
}
