using CategoryCore.Contract.Commands;
using CategoryCore.Contract.Interfaces;
using Infrastructure.Extensions;
using Infrastructure.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Web.Core.Attributes;
using Web.Models;
using YearlyPlanning.Contract.Interfaces;

namespace Web.Controllers
{
    [RoutePrefix("api/category")]
    public class CategoryController : ApiController
    {
        private readonly IMessageBus messageBus;
        private readonly ICategoryService categoryService;
        private readonly IJobService jobService;

        public CategoryController(
            IMessageBus messageBus,
            ICategoryService categoryService,
            IJobService jobService)
        {
            this.messageBus = messageBus;
            this.categoryService = categoryService;
            this.jobService = jobService;
        }

        [CompressFilter]
        [HttpGet, Route("get")]
        public IEnumerable<CategoryViewModel> Get(bool includeGroupedTasks, bool includeHiddenTasks, bool showTasks)
        {
            var jobList = new List<IJob>();
            List<ICategoryModel> categoryTree = categoryService.GetTree().ToList();
            IEnumerable<Guid> categoryIds = categoryTree.Flatten(c => c.Children).Select(c => c.Id);

            if (showTasks)
            {
                List<IJob> jobs = jobService.GetByCategoryIds(categoryIds, includeGroupedTasks, includeHiddenTasks);
                jobList.AddRange(jobs);
            }

            IEnumerable<CategoryViewModel> result = FillCategories(categoryTree, jobList);

            return result;
        }

        [HttpPost, Route("save")]
        public Task Save(CategoryViewModel model)
        {
            if (!model.Id.HasValue)
            {
                var id = Guid.NewGuid();
                return messageBus.Publish(new CreateCategory(id, model.ParentId, model.Name, model.Color, visible: true));
            }
            else
            {
                return messageBus.Publish(new UpdateCategory(model.Id.Value, model.Color, model.Name));
            }
        }

        [HttpPost, Route("hide")]
        public Task Hide(CategoryViewModel model)
        {
            return messageBus.Publish(new HideCategory(model.Id.Value));
        }

        [HttpPost, Route("show")]
        public Task Show(CategoryViewModel model)
        {
            return messageBus.Publish(new ShowCategory(model.Id.Value));
        }

        private IEnumerable<CategoryViewModel> FillCategories(IEnumerable<ICategoryModel> categoryTree, List<IJob> tasks)
        {
            return categoryTree.Select(c =>
            {
                var categoryView = c.Map<CategoryViewModel>();
                var categoryTasks = tasks.FindAll(t => t.CategoryId == c.Id);
                categoryView.CanBeHidden = !categoryTasks.Any();
                categoryView.Parent = c.Parent.Map<CategoryViewModel>();
                categoryView.Children = FillCategories(c.Children, tasks);
                categoryView.Tasks = categoryTasks.OrderBy(i => i.Title).ThenBy(i => i.FirstAddress);

                return categoryView;
            });
        }
    }
}