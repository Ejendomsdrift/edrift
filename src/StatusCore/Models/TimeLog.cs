using System;
using StatusCore.Contract.Interfaces;

namespace StatusCore.Models
{
    public class TimeLog : ITimeLog
    {
        public Guid MemberId { get; set; }

        public TimeSpan SpentTime { get; set; }
    }
}
