using System;

namespace Common.Helpers
{
    public class StringHelper
    {
        private static readonly Random _random;
        static StringHelper()
        {
            _random = new Random();
        }

        public static int GetInteger(int min, int max)
        {
            var value = _random.Next(min, max);
            return value;
        }
    }
}
