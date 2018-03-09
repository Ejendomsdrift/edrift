using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CommonConstants = Infrastructure.Constants.Constants.Common;

namespace Infrastructure.Extensions
{
    public static class DateTimeExtensions
    {
        public static int DaysInMonth(this DateTime value)
        {
            return DateTime.DaysInMonth(value.Year, value.Month);
        }

        public static int GetWeekNumber(this DateTime value)
        {
            var currentCultureInfo = CultureInfo.CurrentCulture;
            var weekNum = currentCultureInfo.Calendar.GetWeekOfYear(value, CalendarWeekRule.FirstFourDayWeek,
                DayOfWeek.Monday);
            return weekNum;
        }

        public static int GetPreviousWeekNumber(this DateTime value)
        {
            return value.GetWeekNumber() - 1;
        }

        public static int? GetWeekDayNumber(this DateTime? value)
        {
            if (value == null) return null;
            var newDate = value ?? DateTime.UtcNow;
            var dayOfWeek = newDate.DayOfWeek;
            return dayOfWeek > 0 ? (int) dayOfWeek : Constants.Constants.DateTime.SundayNumber;
        }

        public static int GetWeekDayNumber(this DateTime value)
        {
            var dayOfWeek = value.DayOfWeek;
            return dayOfWeek > 0 ? (int)dayOfWeek : Constants.Constants.DateTime.SundayNumber;
        }

        public static DateTime SetToLastTickOfDay(this DateTime value)
        {
            return value.AddDays(1).AddTicks(-1);
        }

        public static decimal? MinutesToHours(this int? minutes)
        {
            if (minutes.HasValue)
            {
                return decimal.Round((decimal)minutes.Value/ CommonConstants.MinutesInOneHour, CommonConstants.DecimalCount, MidpointRounding.AwayFromZero);
            }
            return null;
        }

        public static decimal? MinutesToHours(this double? minutes)
        {
            if (minutes.HasValue)
            {
                return decimal.Round((decimal)minutes.Value / CommonConstants.MinutesInOneHour, CommonConstants.DecimalCount, MidpointRounding.AwayFromZero);
            }
            return null;
        }

        public static decimal MinutesToHours(this double minutes)
        {
            return decimal.Round((decimal)minutes / CommonConstants.MinutesInOneHour, CommonConstants.DecimalCount, MidpointRounding.AwayFromZero);
        }

        public static int HoursToMinutes(this decimal hours)
        {
            return (int)TimeSpan.FromHours((double)hours).TotalMinutes;
        }

        public static TimeSpan Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, TimeSpan> selector)
        {
            return source.Select(selector).Aggregate(TimeSpan.Zero, (t1, t2) => t1.Add(t2));
        }
    }
}