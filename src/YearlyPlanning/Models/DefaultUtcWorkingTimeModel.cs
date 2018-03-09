using System;
using Infrastructure.Constants;
using Infrastructure.Helpers;

namespace YearlyPlanning.Models
{
    public class DefaultUtcWorkingTimeModel
    {
        public int DefaultTaskStartUtcHour { get; set; }
        public DateTime DefaultUtcWorkStartTime { get; set; }
        public DateTime DefaultUtcWorkFinishTime { get; set; }

        public DefaultUtcWorkingTimeModel(IAppSettingHelper appSettingHelper, DateTime date, double workingHours)
        {
            DefaultTaskStartUtcHour = appSettingHelper.GetAppSetting<int>(Constants.AppSetting.DefaultTaskStartUtcTime);
            DefaultUtcWorkStartTime = new DateTime(date.Year, date.Month, date.Day, DefaultTaskStartUtcHour, 0, 0, DateTimeKind.Utc);
            DefaultUtcWorkFinishTime = DefaultUtcWorkStartTime.AddHours(workingHours);
        }
    }
}
