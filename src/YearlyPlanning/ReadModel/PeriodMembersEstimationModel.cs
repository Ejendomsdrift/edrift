using System;
using System.Collections.Generic;
using YearlyPlanning.Contract.Interfaces;

namespace YearlyPlanning.ReadModel
{
    public class PeriodMembersEstimationModel
    {
        public IDictionary<Guid, IEnumerable<IDayAssign>> Assigns { get; set; }
        public IDictionary<Guid, IEnumerable<Guid>> UserInGroups { get; set; }
        public int UsersCount { get; set; }
    }
}
