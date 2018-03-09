using System;
using YearlyPlanning.Contract.Enums;

namespace YearlyPlanning.Contract.Models
{
    public abstract class ChangeJobAssignAbstractModel
    {
        public string JobId { get; set; }
        public Guid AssignId { get; set; }
        public Guid DepartmentId { get; set; }
        public ChangedByRole ChangedByRole { get; set; }
    }
}
