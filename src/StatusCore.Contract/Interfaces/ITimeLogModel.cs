using System;

namespace StatusCore.Contract.Interfaces
{
    public interface ITimeLogModel
    {
        Guid MemberId { get; set; }

        TimeSpan SpentTime { get; set; }
    }
}
