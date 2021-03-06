﻿using System;
using System.Collections.Generic;
using Infrastructure.EventSourcing.Implementation;
using MemberCore.Contract.Enums;
using YearlyPlanning.Contract.Models;

namespace YearlyPlanning.Contract.Events.JobAssignEvents
{
    public class AdHockJobAssignCreatedEvent: EventBase
    {
        public int TillYear { get; set; }

        public IEnumerable<WeekModel> WeekList { get; set; }

        public Guid DepartmentId { get; set; }

        public int RepeatsPerWeek { get; set; }        

        public string Description { get; set; }

        public List<string> JobIdList { get; set; } = new List<string>();

        public RoleType CreatedByRole { get; set; }

        public bool IsGlobal { get; set; }

        public bool IsEnabled { get; set; }
    }
}
