using System;
using System.Collections.Generic;

namespace YearlyPlanning.Contract.Commands.DayAssignCommands
{
    public class ChangeDayAssignMembersComand : DayAssignCommand
    {
        public Guid? GroupId { get; set; }
        public string GroupName { get; set; }
        public List<Guid> UserIdList { get; set; }
        public Guid? TeamLeadId { get; set; }
        public bool IsAssignedToAllUsers { get; set; }

        public ChangeDayAssignMembersComand() { }
        public ChangeDayAssignMembersComand(string id) : base(id)
        {
        }
    }
}