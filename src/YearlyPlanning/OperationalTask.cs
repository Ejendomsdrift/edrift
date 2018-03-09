using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.EventSourcing.Implementation;
using YearlyPlanning.Contract.Events.OperationalTaskEvents;

namespace YearlyPlanning
{
    public class OperationalTask : AggregateBase
    {
        public int Year { get; set; }

        public int Week { get; set; }

        public Guid CategoryId { get; set; }

        public Guid DepartmentId { get; set; }

        public IEnumerable<int> DaysPerWeek { get; set; } = Enumerable.Empty<int>();

        public decimal Estimate { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Address { get; set; }

        public List<Guid> AssignedEmployees { get; set; }

        public Guid GroupId { get; set; }

        public OperationalTask()
        {
            RegisterTransition<OperationalTaskCreatedEvent>(Apply);
            RegisterTransition<AdHocTaskSaveDaysPerWeekEvent>(Apply);
            RegisterTransition<OperationalTaskChangeEstimateEvent>(Apply);
            RegisterTransition<OperationalTaskChangeDescriptionEvent>(Apply);
            RegisterTransition<OperationalTaskChangeAddress>(Apply);
            RegisterTransition<OperationalTaskSaveAssignEmployeesEvent>(Apply);
            RegisterTransition<AdHocTaskChangeCategoryEvent>(Apply);
            RegisterTransition<OperationalTaskChangeTitleEvent>(Apply);
        }

        public OperationalTask(string id, int year, int week, Guid categoryId, Guid departmentId, string title) : this()
        {
            Id = id;

            RaiseEvent(new OperationalTaskCreatedEvent
            {
                Year = year,
                Week = week,
                CategoryId = categoryId,
                DepartmentId = departmentId,
                Title = title,
            });
        }

        private void Apply(OperationalTaskCreatedEvent e)
        {
            Id = e.SourceId;
            Year = e.Year;
            Week = e.Week;
            CategoryId = e.CategoryId;
            DepartmentId = e.DepartmentId;
            Title = e.Title;
        }

        public void SaveDaysPerWeek(IEnumerable<int> daysPerWeek)
        {
            RaiseEvent(new AdHocTaskSaveDaysPerWeekEvent { DaysPerWeek = daysPerWeek });
        }

        private void Apply(AdHocTaskSaveDaysPerWeekEvent e)
        {
            DaysPerWeek = e.DaysPerWeek;
        }

        public void ChangeEstimate(decimal estimate)
        {
            RaiseEvent(new OperationalTaskChangeEstimateEvent { Estimate = estimate });
        }

        private void Apply(OperationalTaskChangeEstimateEvent e)
        {
            Estimate = e.Estimate;
        }

        public void ChangeDescription(string description)
        {
            RaiseEvent(new OperationalTaskChangeDescriptionEvent { Description = description });
        }

        private void Apply(OperationalTaskChangeDescriptionEvent e)
        {
            Description = e.Description;
        }

        public void ChangeAddress(string address)
        {
            RaiseEvent(new OperationalTaskChangeAddress { Address = address });
        }

        private void Apply(OperationalTaskChangeAddress e)
        {
            Address = e.Address;
        }

        public void ChangeAssignedEmployees(Guid groupId, IEnumerable<Guid> employees)
        {
            RaiseEvent(new OperationalTaskSaveAssignEmployeesEvent { GroupId = groupId, AssignedEmployees = employees });
        }

        private void Apply(OperationalTaskSaveAssignEmployeesEvent e)
        {
            AssignedEmployees = e.AssignedEmployees.ToList();
            GroupId = e.GroupId;
        }

        public void ChangeCategory(Guid categoryId)
        {
            RaiseEvent(new AdHocTaskChangeCategoryEvent { CategoryId = categoryId });
        }

        private void Apply(AdHocTaskChangeCategoryEvent e)
        {
            CategoryId = e.CategoryId;
        }

        public void ChangeTitle(string title)
        {
            RaiseEvent(new OperationalTaskChangeTitleEvent { Title = title });
        }

        private void Apply(OperationalTaskChangeTitleEvent e)
        {
            Title = e.Title;
        }

        public static OperationalTask Create(string id, int year, int week, Guid categoryId, Guid departmentId, string title)
        {
            return new OperationalTask(id, year, week, categoryId, departmentId, title);
        }
    }
}
