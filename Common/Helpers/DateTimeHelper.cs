using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Common.Helpers
{
    public static class DateTimeHelper
    {
        public static int WeekOfYear(this DateTimeOffset dateTimeOffset, CalendarWeekRule calendarWeekRule = CalendarWeekRule.FirstDay, DayOfWeek dayOfWeek = DayOfWeek.Sunday)
        {
            var weekOfYear = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(dateTimeOffset.Date, calendarWeekRule, dayOfWeek);
            return weekOfYear;
        }

        public static DateTimeOffset StartOfWeek(this DateTimeOffset dateTimeOffset, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dateTimeOffset.DayOfWeek - startOfWeek)) % 7;
            return dateTimeOffset.AddDays(-1 * diff).Date;
        }

        public static DateTime StartDateOfWeek(int weekOfYear, DateTime selectedDate, CalendarWeekRule calendarWeekRule = CalendarWeekRule.FirstDay, DayOfWeek dayOfWeek = DayOfWeek.Sunday)
        {
            var firstDayOfWeek = new DateTime(selectedDate.Year, selectedDate.Month, 1);
            while (weekOfYear != CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(firstDayOfWeek, calendarWeekRule, dayOfWeek))
            {
                firstDayOfWeek = firstDayOfWeek.AddDays(1);
            }

            return firstDayOfWeek;
        }

        public static DateTime EndDateOfWeek(int weekOfYear, DateTime selectedDate, CalendarWeekRule calendarWeekRule = CalendarWeekRule.FirstDay, DayOfWeek dayOfWeek = DayOfWeek.Sunday)
        {
            // TODO: Revisit the logic
            var firstDayOfWeek = StartDateOfWeek(weekOfYear, selectedDate, calendarWeekRule, dayOfWeek);

            var day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(firstDayOfWeek);

            var dayNumber = (int)day;

            // Since start day number is 0.
            var lastDayNumber = 5 - dayNumber;

            var lastDayOfWeek = firstDayOfWeek.AddDays(lastDayNumber);
            return lastDayOfWeek;
        }

        public static List<DateTimeOffset> ListDates(DateTimeOffset startDate)
        {
            var dates = new List<DateTimeOffset>();
            do
            {
                dates.Add(startDate);
                startDate = startDate.AddDays(1);
            } while (startDate.Date <= DateTimeOffset.UtcNow.Date);

            return dates;
        }

        public static List<DateTimeOffset> ListDates(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            var dates = new List<DateTimeOffset>();
            do
            {
                dates.Add(startDate);
                startDate = startDate.AddDays(1);
            } while (startDate.Date <= endDate.Date);

            return dates;
        }

        public static IDictionary<DateTimeOffset, DateTimeOffset> ListWeekRange(DateTimeOffset startDate, DateTimeOffset endDate, bool isOverrideStartOfWeek = true)
        {
            var dates = ListDates(startDate, endDate);
            var weekRanges = new Dictionary<DateTimeOffset, DateTimeOffset>();

            for (int i = 0; i < dates.Count; i++)
            {
                var startOfWeek = dates[i].StartOfWeek(DayOfWeek.Monday);
                var day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(startOfWeek.Date);
                var endOfWeek = dates[i].AddDays(5 - (int)day);

                var weekDay = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(dates[i].Date);

                if (i == 0 && isOverrideStartOfWeek)
                {
                    day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(dates[i].Date);
                    endOfWeek = dates[i].AddDays(5 - (int)day);
                    weekRanges.Add(dates[i], endOfWeek);
                }
                else if (!weekRanges.Any(a => a.Key <= dates[i] && a.Value >= dates[i]) && !new int[] { 0, 6 }.Contains((int)weekDay))
                {
                    weekRanges.Add(startOfWeek, endOfWeek);
                }
            }

            return weekRanges;
        }
    }
}
