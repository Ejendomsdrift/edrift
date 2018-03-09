namespace Web.Models
{
    public class TimeViewModel
    {
        public int ScheduledMinutes { get; set; }
        public int WorkingMinutes { get; set; }
        public bool IsAbsent { get; set; }

        public static TimeViewModel operator +(TimeViewModel tv1, TimeViewModel tv2)
        {
            return new TimeViewModel
            {
                WorkingMinutes = tv1.WorkingMinutes + tv2.WorkingMinutes,
                ScheduledMinutes = tv1.ScheduledMinutes + tv2.ScheduledMinutes
            };
        }
    }
}