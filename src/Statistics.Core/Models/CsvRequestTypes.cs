using System;
using System.Collections.Generic;
using Statistics.Contract.Interfaces.Models;

namespace Statistics.Core.Models
{
    public class CsvRequest : ICsvRequest
    {
        public string RangeDateString { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class TasksCsvRequest : CsvRequest, ITasksCsvRequest
    {
        public IEnumerable<Guid> TasksIds { get; set; }
    }

    public class GroupedTasksCsvRequest<T> : CsvRequest, IGroupedTasksCsvRequest<T>
    {
        public IDictionary<T, IEnumerable<Guid>> GroupedTasksIds { get; set; }
    }

    public class AddressStatisticsCsvRequest : CsvRequest, IAddressStatisticsCsvRequest
    {
        public IEnumerable<AddressStatisticInfo> AddressStatisticInfos { get; set; }
    }

    public class AbsencesStatisticsCsvRequest : CsvRequest, IAbsencesStatisticsCsvRequest
    {
        public IEnumerable<Guid> AbsencesIdList { get; set; }
    }

    public class CancelingReasonInfoRequest : CsvRequest, ICancelingReasonInfoRequest
    {
        public IEnumerable<Guid> DayAssignIdList { get; set; }
    }
}