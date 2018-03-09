using System;

namespace StatusCore.Contract.Interfaces
{
    public interface IMemberSpentTimeModel
    {
        Guid MemberId { get; set; }

        int SpentHours { get; set; }

        int SpentMinutes { get; set; }
    }
}
