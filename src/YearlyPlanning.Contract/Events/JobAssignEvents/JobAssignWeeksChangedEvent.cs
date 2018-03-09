﻿using System.Collections.Generic;
using Infrastructure.EventSourcing.Implementation;
using YearlyPlanning.Contract.Enums;
using YearlyPlanning.Contract.Models;

namespace YearlyPlanning.Contract.Events.JobAssignEvents
{
    public class JobAssignWeeksChangedEvent : EventBase
    {
        public string DepartmentId { get; set; }

        public IEnumerable<WeekModel> Weeks { get; set; }

        public ChangedByRole ChangedByRole { get; set; }

        public bool IsLocalIntervalChanged { get; set; }
    }
}
