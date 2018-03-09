using System;
using StatusCore.Contract.Interfaces;

namespace Web.Models
{
    public class MemberSpentTimeModel: IMemberSpentTimeModel
    {
        public Guid MemberId { get; set; }

        public int SpentHours { get; set; }

        public int SpentMinutes { get; set; }
    }
}