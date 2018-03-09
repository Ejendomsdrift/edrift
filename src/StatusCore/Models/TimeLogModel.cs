using System;
using StatusCore.Contract.Interfaces;

namespace StatusCore.Models
{
    public class TimeLogModel: ITimeLogModel
    {
        public Guid MemberId { get; set; }

        public TimeSpan SpentTime { get; set; }

    }
}
