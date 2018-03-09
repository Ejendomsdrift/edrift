using System;
using System.Threading.Tasks;
using CategoryCore.Contract.Events;
using CategoryCore.Models;
using Infrastructure.Messaging;
using MongoRepository.Contract.Interfaces;

namespace CategoryCore.Handlers
{
    public class CategoryModelGenerator :
        IHandler<CategoryNameSet>,
        IHandler<CategoryColorSet>,
        IHandler<CategoryCreated>,
        IHandler<CategoryHidden>,
        IHandler<CategoryShown>
    {

        private readonly IRepository<Category> categoryRepository;
        public CategoryModelGenerator(IRepository<Category> categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        public async Task Handle(CategoryCreated message)
        {
            var category = new Category();
            category.Id = Guid.Parse(message.SourceId);
            category.ParentId = message.ParentId;
            category.Name = message.Name;
            category.Color = message.Color;
            category.Visible = message.Visible;

            categoryRepository.Save(category);
        }

        public async Task Handle(CategoryNameSet message)
        {
            categoryRepository.UpdateSingleProperty(Guid.Parse(message.SourceId), c=>c.Name, message.Name);
        }

        public async Task Handle(CategoryColorSet message)
        {
            categoryRepository.UpdateSingleProperty(Guid.Parse(message.SourceId), c => c.Color, message.Color);
        }

        public async Task Handle(CategoryHidden message)
        {
            categoryRepository.UpdateSingleProperty(Guid.Parse(message.SourceId), c => c.Visible, false);
        }

        public async Task Handle(CategoryShown message)
        {
            categoryRepository.UpdateSingleProperty(Guid.Parse(message.SourceId), c => c.Visible, true);
        }
    }
}