using System;
using Infrastructure.Extensions;
using NUnit.Framework;

namespace Infrastructure.Tests.Extensions
{
    [TestFixture]
    class DateTimeExtensionsTests
    {
        [Test]
        public void WeekDayNumber_Test()
        {
            //arrange
            var date = DateTime.SpecifyKind(new DateTime(2017, 1, 1), DateTimeKind.Utc);
            //act
            var result = date.GetWeekDayNumber();
            //arrange
            Assert.AreEqual(result, Constants.Constants.DateTime.SundayNumber);
        }

        [Test]
        public void DaysInMonth_Test()
        {
            //arrange
            var date = DateTime.SpecifyKind(new DateTime(2017, 1, 1), DateTimeKind.Utc);
            //act
            var result = date.DaysInMonth();
            //arrange
            Assert.AreEqual(result, 31);
        }

        [Test]
        public void WeekNumber_Test()
        {
            //arrange
            var date = DateTime.SpecifyKind(new DateTime(2017, 1, 14), DateTimeKind.Utc);
            //act
            var result = date.GetWeekNumber();
            //arrange
            Assert.AreEqual(result, 2);
        }

        [Test]
        public void GetWeekDayNumber_Test()
        {
            //arrange
            var date = DateTime.SpecifyKind(new DateTime(2017, 1, 14), DateTimeKind.Utc);
            //act
            var result = date.GetWeekDayNumber();
            //arrange
            Assert.AreEqual(result, 6);
        }

        [Test]
        public void GetWeekDayNumber_DateIsNull_Test()
        {
            //arrange
            DateTime? date = null;
            //act
            var result = date.GetWeekDayNumber();
            //arrange
            Assert.IsFalse(result.HasValue);
        }
    }
}