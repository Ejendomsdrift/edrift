using System;
using System.Threading.Tasks;
using Infrastructure.EventSourcing;
using Infrastructure.EventSourcing.Exceptions;
using Infrastructure.Messaging;
using YearlyPlanning.Contract.Commands.OperationalTaskCommands;

namespace YearlyPlanning.Handlers
{
    public class OperationalTaskCommandHandler :
        IHandler<CreateOperationalTaskCommand>,
        IHandler<ChangeOperationalTaskEstimateCommand>,
        IHandler<ChangeOperationalTaskDescriptionCommand>,
        IHandler<ChangeAdHocTaskCategoryCommand>,
        IHandler<ChangeAdHocTaskDayPerWeekCommand>,
        IHandler<ChangeOperationalTaskTitleCommand>,
        IHandler<ChangeOperationalTaskAssignsEmployeesCommand>,
        IHandler<UnassignOperationalTaskEmployeesCommand>
    {
        private readonly IAggregateRootRepository<OperationalTask> repository;

        public OperationalTaskCommandHandler(IAggregateRootRepository<OperationalTask> repository)
        {
            this.repository = repository;
        }

        public async Task Handle(CreateOperationalTaskCommand message)
        {
            try
            {
                var item = await repository.Get(message.Id.ToString());
                if (item != null)
                {
                    throw new Exception($"Operational task with id: {message.Id} already exist");
                }
            }
            catch (AggregateNotFoundException)
            {
                // That is fine that id not used
            }

            var operationalTask = OperationalTask.Create(message.Id, message.Year, message.Week, message.CategoryId, message.DepartmentId, message.Title);
            operationalTask.SaveDaysPerWeek(message.DaysPerWeek);
            operationalTask.ChangeAssignedEmployees(message.GroupId, message.AssignedEmployees);
            await repository.Save(operationalTask);
        }

        public async Task Handle(ChangeOperationalTaskEstimateCommand message)
        {
            var operationalTask = await repository.Get(message.Id);
            operationalTask.ChangeEstimate(message.Estimate);
            await repository.Save(operationalTask);
        }

        public async Task Handle(ChangeOperationalTaskDescriptionCommand message)
        {
            var operationalTask = await repository.Get(message.Id);
            operationalTask.ChangeDescription(message.Description);
            await repository.Save(operationalTask);
        }

        public async Task Handle(ChangeAdHocTaskCategoryCommand message)
        {
            var operationalTask = await repository.Get(message.Id);
            operationalTask.ChangeCategory(message.CategoryId);
            await repository.Save(operationalTask);
        }

        public async Task Handle(ChangeAdHocTaskDayPerWeekCommand message)
        {
            var operationalTask = await repository.Get(message.Id);
            operationalTask.SaveDaysPerWeek(message.DaysPerWeek);
            await repository.Save(operationalTask);
        }

        public async Task Handle(ChangeOperationalTaskTitleCommand message)
        {
            var operationalTask = await repository.Get(message.Id);
            operationalTask.ChangeTitle(message.Title);
            await repository.Save(operationalTask);
        }

        public async Task Handle(ChangeOperationalTaskAssignsEmployeesCommand message)
        {
            var operationalTask = await repository.Get(message.Id);
            operationalTask.ChangeAssignedEmployees(message.GroupId, message.AssignedEmployees);
            await repository.Save(operationalTask);
        }

        public async Task Handle(UnassignOperationalTaskEmployeesCommand message)
        {
            var operationalTask = await repository.Get(message.Id);
            operationalTask.ChangeAssignedEmployees(message.GroupId, message.Employees);
            await repository.Save(operationalTask);
        }
    }
}
