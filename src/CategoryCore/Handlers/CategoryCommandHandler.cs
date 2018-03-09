using System;
using System.Threading.Tasks;
using CategoryCore.Contract.Commands;
using CategoryCore.Models;
using Infrastructure.EventSourcing;
using Infrastructure.EventSourcing.Exceptions;
using Infrastructure.Messaging;

namespace CategoryCore.Handlers
{
    public class CategoryCommandHandler :
        IHandler<CreateCategory>,
        IHandler<UpdateCategory>,
        IHandler<HideCategory>,
        IHandler<ShowCategory>
    {
        private readonly IAggregateRootRepository<CategorySource> repository;

        public CategoryCommandHandler(IAggregateRootRepository<CategorySource> repository)
        {
            this.repository = repository;
        }

        public async Task Handle(CreateCategory message)
        {
            try
            {
                var item = await repository.Get(message.Id.ToString());
                if (item != null)
                {
                    throw new Exception($"Category with id: {message.Id} already exist");
                }
            }
            catch (AggregateNotFoundException)
            {
                // That is fine that id not used
            }
            var category = CategorySource.Create(message.Id, message.ParentId, message.Name, message.Color, message.Visible);
            await repository.Save(category);
        }

        public async Task Handle(UpdateCategory message)
        {
            var category = await repository.Get(message.Id.ToString());

            if (!string.Equals(category.Name, message.Name, StringComparison.InvariantCulture))
            {
                category.SetName(message.Name);
            }
            if (!string.Equals(category.Color, message.Color, StringComparison.InvariantCultureIgnoreCase))
            {
                category.SetColor(message.Color);
            }

            await repository.Save(category);
        }

        public async Task Handle(HideCategory message)
        {
            var category = await repository.Get(message.Id.ToString());
            category.Hide();
            await repository.Save(category);
        }

        public async Task Handle(ShowCategory message)
        {
            var category = await repository.Get(message.Id.ToString());
            category.Show();
            await repository.Save(category);
        }
    }
}