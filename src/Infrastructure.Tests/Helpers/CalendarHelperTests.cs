using System;
using System.Globalization;
using System.Linq;
using Infrastructure.Helpers.Implementation;
using NUnit.Framework;

namespace Infrastructure.Tests.Helpers
{
    [TestFixture]
    public class CalendarHelperTests
    {
        [Test]
        public void GetTotalWeeks_Test()
        {
            //act
            var result = CalendarHelper.GetTotalWeeks().ToArray();
            //assert
            Assert.IsTrue(result.Any());
            Assert.IsTrue(result[0] == 1);
            if (result.Length > 1)
            {
                for (var i = 1; i < result.Length; i++)
                {
                    Assert.IsTrue(result[i] - result[i - 1] == 1);
                }
            }
        }

        [Test]
        public void GetFirstDayOfWeek_Test()
        {
            //arrange
            const int year = 2017;
            const int week = 2;
            const int expectedDay = 9;
            //act
            var result = CalendarHelper.GetFirstDayOfWeek(year, week);
            //assert
            Assert.AreEqual(result.DayOfWeek, DayOfWeek.Monday);
            Assert.IsTrue(result.Day == expectedDay);
        }

        [Test]
        public void GetWeekDays_Test()
        {
            //arrange
            const int year = 2017;
            const int week = 2;
            //act
            var result = CalendarHelper.GetWeekDays(year, week);
            //assert
            Assert.AreEqual(result.Count, Constants.Constants.DateTime.DaysInWeek);
        }


        [Test]
        public void GetYearWeeks_Test()
        {
            //arrange
            var date = DateTime.SpecifyKind(new DateTime(2017, 5, 1), DateTimeKind.Utc);
            var monthName = date.ToString("MMM", CultureInfo.InvariantCulture);
            const int exeptedWeekCount = 4;
            //act
            var result = CalendarHelper.GetYearWeeks(date.Year);
            //assert
            Assert.AreEqual(result.FirstOrDefault(m => m.MonthName == monthName).WeekCount, exeptedWeekCount);
        }
    }
}