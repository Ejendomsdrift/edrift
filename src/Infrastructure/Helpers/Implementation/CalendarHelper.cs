using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Infrastructure.Extensions;
using Infrastructure.Models;

namespace Infrastructure.Helpers.Implementation
{
    public static class CalendarHelper
    {
        public static List<MonthWeeksModel> GetYearWeeks(int year)
        {
            var result = new List<MonthWeeksModel>();
            var firstDateOfYear = DateTime.SpecifyKind(new DateTime(year, 1, 1), DateTimeKind.Utc);
            var tempWeekDay = firstDateOfYear.GetWeekDayNumber();

            for (var i = 1; i <= Constants.Constants.DateTime.MonthsInYear; i++)
            {
                var monthWeeksModel = new MonthWeeksModel();

                var month = DateTime.SpecifyKind(new DateTime(year, i, 1), DateTimeKind.Utc);
                monthWeeksModel.MonthName = month.ToString("MMM", CultureInfo.InvariantCulture);
                var daysInMonth = month.DaysInMonth();

                var daysToWeekEnd = tempWeekDay > 1 ? DaysToWeekEnd(tempWeekDay) : 0;
                monthWeeksModel.WeekCount += (daysInMonth - daysToWeekEnd) / Constants.Constants.DateTime.DaysInWeek;
                var nextMonthStartWeekDay = daysInMonth - daysToWeekEnd -
                                            monthWeeksModel.WeekCount * Constants.Constants.DateTime.DaysInWeek + 1;

                if (tempWeekDay <= Constants.Constants.DateTime.BoundaryDayNumber && tempWeekDay > 1)
                {
                    monthWeeksModel.WeekCount++;
                }

                if (nextMonthStartWeekDay > Constants.Constants.DateTime.BoundaryDayNumber)
                {
                    if (nextMonthStartWeekDay > 8)
                    {
                        throw new ArgumentException("wrong week calculation");
                    }

                    monthWeeksModel.WeekCount++;
                }

                if (i == Constants.Constants.DateTime.MonthsInYear)
                {
                    monthWeeksModel.WeekCount =
                        GetCorrectLastMonthWeeks(result.Sum(x => x.WeekCount) + monthWeeksModel.WeekCount,
                            monthWeeksModel.WeekCount);
                }

                result.Add(monthWeeksModel);

                tempWeekDay = nextMonthStartWeekDay;
            }

            return result;
        }

        public static IEnumerable<int> GetTotalWeeks()
        {
            return Enumerable.Range(1, Constants.Constants.DateTime.WeeksInYear);
        }

        public static List<WeekDayModel> GetWeekDays(int year, int week)
        {
            var currentDate = DateTime.UtcNow;
            var monday = GetFirstDayOfWeek(year, week);

            var result = new List<WeekDayModel>();
            for (var i = 0; i < Constants.Constants.DateTime.DaysInWeek; i++)
            {
                var day = monday.AddDays(i);
                result.Add(new WeekDayModel
                {
                    DayNumber = i + 1,
                    DayOfWeek = day.DayOfWeek.ToString(),
                    Date = day,
                    IsCurrent = day.Date == currentDate.Date
                });
            }
            return result;
        }

        public static DateTime GetFirstDayOfWeek(int year, int week)
        {
            var cultureInfo = CultureInfo.CurrentCulture;
            var jan1 = DateTime.SpecifyKind(new DateTime(year, 1, 1), DateTimeKind.Utc);
            var daysOffset = (int)DayOfWeek.Monday - (int)jan1.DayOfWeek;
            var firstWeekDay = jan1.AddDays(daysOffset);
            var firstWeek = cultureInfo.Calendar.GetWeekOfYear(jan1, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            if ((firstWeek <= 1 || firstWeek >= Constants.Constants.DateTime.WeeksInYear) && daysOffset >= -3)
            {
                week -= 1;
            }
            return firstWeekDay.AddDays(week * Constants.Constants.DateTime.DaysInWeek);
        }

        public static DateTime GetDateByWeekAndDayNumber(int year, int weekNumber, int dayNumber)
        {
            DateTime firstDayOfWeek = GetFirstDayOfWeek(year, weekNumber);
            return DateTime.SpecifyKind(firstDayOfWeek.AddDays(dayNumber - 1), DateTimeKind.Utc);
        }

        public static int GetWeekNumber(DateTime date)
        {
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(date);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                date = date.AddDays(3);
            }

            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        public static IEnumerable<DateTime> GetDatesRange(DateTime startDate, DateTime endDate)
        {
            while (startDate <= endDate)
            {
                yield return startDate;
                startDate = startDate.AddDays(1);
            }
        }

        public static IDictionary<DateTime, int> GetDateWithWeekDayListForPeriod(DateTime startDate, DateTime endDate)
        {
            return GetDatesRange(startDate, endDate).ToDictionary(day => day, day => day.GetWeekDayNumber());
        }

        public static bool IsWeekend(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday || date.Date.DayOfWeek == DayOfWeek.Sunday;
        }

        public static bool IsDatesIsSameDay(DateTime firstDate, DateTime secondDate)
        {
            return firstDate.Date == secondDate.Date;
        }

        private static int GetCorrectLastMonthWeeks(int totalWeekCount, int lastMonthtWeekCount)
        {
            return totalWeekCount <= Constants.Constants.DateTime.WeeksInYear
                ? lastMonthtWeekCount
                : lastMonthtWeekCount - (totalWeekCount - Constants.Constants.DateTime.WeeksInYear);
        }

        private static int DaysToWeekEnd(int weekDayNumber)
        {
            return Constants.Constants.DateTime.DaysInWeek - weekDayNumber + 1;
        }
    }
}