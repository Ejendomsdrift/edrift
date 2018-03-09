using System;
using System.Collections.Generic;

namespace StatusCore.Contract.Interfaces
{
    public interface IChangeJobStatusModel
    {
        Guid DayAssignId { get; set; }

        string ChangeStatusComment { get; set; }

        List<IMemberSpentTimeModel> Members { get; set; }

    }
}
