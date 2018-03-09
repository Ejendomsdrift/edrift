using System;

namespace StatusCore.Contract.Interfaces
{
    public interface ITimeLog
    {
        Guid MemberId { get; set; }

        TimeSpan SpentTime { get; set; }
    }
}
