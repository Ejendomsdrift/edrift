using System;
using System.Collections.Generic;

namespace Statistics.Contract.Interfaces.Models
{
    public interface ICsvRequest
    {
        string RangeDateString { get; set; }
        DateTime StartDate { get; set; }
        DateTime EndDate { get; set; }
    }

    public interface IGroupedTasksCsvRequest<T> : ICsvRequest
    {
        IDictionary<T, IEnumerable<Guid>> GroupedTasksIds { get; set; }
    }

    public interface ITasksCsvRequest : ICsvRequest
    {
        IEnumerable<Guid> TasksIds { get; set; }
    }

    public interface IAddressStatisticsCsvRequest : ICsvRequest
    {
        IEnumerable<AddressStatisticInfo> AddressStatisticInfos { get; set; }
    }

    public interface IAbsencesStatisticsCsvRequest : ICsvRequest
    {
        IEnumerable<Guid> AbsencesIdList { get; set; }
    }

    public interface ICancelingReasonInfoRequest: ICsvRequest
    {
        IEnumerable<Guid> DayAssignIdList { get; set; }
    }


}