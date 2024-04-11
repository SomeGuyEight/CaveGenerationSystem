using System;

namespace SlimeGame 
{
    public static class SGUtils 
    {
        public static string DateTimeStamp(DateTime? dateTime = null)
        {
            return DateStamp(dateTime) + " " + TimeStamp(dateTime);
        }
        public static string DateStamp(DateTime? dateTime = null)
        {
            var now = dateTime ?? DateTime.Now;
            string month  = Format(now.Month  );
            string day    = Format(now.Day    );
            return $"({month}-{day}-{now.Year})";
        }
        public static string TimeStamp(DateTime? dateTime = null)
        {
            var now = dateTime ?? DateTime.Now;
            string hour   = Format(now.Hour   );
            string minute = Format(now.Minute );
            string second = Format(now.Second );
            return $"({hour};{minute};{second})";
        }
        private static string Format(int value)
        {
            return value > 9 ? $"{value}" : "0" + value;
        }

        public static bool IsPowerOfTwo(this int x) 
        {
            return (x != 0) && ((x & (x - 1)) == 0);
        }

    }
}
