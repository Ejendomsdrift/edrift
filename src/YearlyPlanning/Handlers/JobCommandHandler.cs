using System;
using System.Threading.Tasks;
using Infrastructure.EventSourcing;
using Infrastructure.EventSourcing.Exceptions;
using Infrastructure.Messaging;
using YearlyPlanning.Contract.Commands.JobCommands;

namespace YearlyPlanning.Handlers
{
    public class JobCommandHandler :
        IHandler<CreateJobCommand>,
        IHandler<ChangeJobCategoryCommand>,
        IHandler<ChangeJobAddressCommand>,
        IHandler<ChangeJobTitleCommand>,
        IHandler<ChangeJobVisibilityCommand>
    {
        private readonly IAggregateRootRepository<JobDomain> repository;

        public JobCommandHandler(IAggregateRootRepository<JobDomain> repository)
        {
            this.repository = repository;
        }

        public async Task Handle(CreateJobCommand message)
        {
            try
            {
                var item = await repository.Get(message.Id);
                if (item != null)
                {
                    throw new Exception($"Operational task with id: {message.Id} already exist");
                }
            }
            catch (AggregateNotFoundException)
            {
                // That is fine that id not used
            }

            var operationalTask = JobDomain.Create(
                message.Id, message.CategoryId, message.Title, message.JobTypeId, message.CreatorId, message.CreatedByRole, message.AddressList, message.RelationGroupList, message.ParentId);
            await repository.Save(operationalTask);
        }

        public async Task Handle(ChangeJobCategoryCommand message)
        {
            var operationalTask = await repository.Get(message.Id);
            operationalTask.ChangeCategory(message.CategoryId);
            await repository.Save(operationalTask);
        }

        public async Task Handle(ChangeJobAddressCommand message)
        {
            var job = await repository.Get(message.Id);
            job.ChangeAddress(message.HousingDepartmentId, message.Address);
            await repository.Save(job);
        }

        public async Task Handle(ChangeJobTitleCommand message)
        {
            var operationalTask = await repository.Get(message.Id);
            operationalTask.ChangeTitle(message.Title);
            await repository.Save(operationalTask);
        }

        public async Task Handle(ChangeJobVisibilityCommand message)
        {
            var operationalTask = await repository.Get(message.Id);
            operationalTask.ChangeVisibility(message.IsHidden);
            await repository.Save(operationalTask);
        }
    }
}