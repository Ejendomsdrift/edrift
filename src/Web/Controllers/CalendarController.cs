using System;
using System.Collections.Generic;
using System.Web.Http;
using Infrastructure.Extensions;
using Infrastructure.Helpers.Implementation;
using Infrastructure.Models;
using Web.Models;

namespace Web.Controllers
{
    [RoutePrefix("api/calendar")]
    public class CalendarController : ApiController
    {
        [HttpGet, Route("GetWeeksModel")]
        public YearWeeksViewModel GetWeeksModel(int year)
        {
            var result = new YearWeeksViewModel();
            result.Year = year;
            result.MonthWeeks = CalendarHelper.GetYearWeeks(year);
            return result;
        }

        [HttpGet, Route("GetTotalWeeks")]
        public TotalWeeksViewModel GetTotalWeeks()
        {
            var result = new TotalWeeksViewModel();
            result.TotalWeeks = CalendarHelper.GetTotalWeeks();
            result.CurrentWeek = DateTime.UtcNow.GetWeekNumber();
            return result;
        }

        [HttpGet, Route("GetWeekDays")]
        public List<WeekDayModel> GetDaysModel(int year, int week)
        {
            return CalendarHelper.GetWeekDays(year, week);
        }

        [HttpGet, Route("GetCurrentWeek")]
        public int GetCurrentWeek()
        {
            return DateTime.UtcNow.GetWeekNumber();
        }
    }
}