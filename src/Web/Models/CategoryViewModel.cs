using System;
using System.Collections.Generic;
using System.Linq;
using YearlyPlanning.Contract.Interfaces;
using YearlyPlanning.ReadModel;

namespace Web.Models
{
    public class CategoryViewModel
    {
        public Guid? Id { get; set; }
        public Guid? ParentId { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public bool Visible { get; set; }
        public bool CanBeHidden { get; set; } = true;
        public CategoryViewModel Parent { get; set; }
        public IEnumerable<CategoryViewModel> Children { get; set; } = Enumerable.Empty<CategoryViewModel>();
        public IEnumerable<IJob> Tasks { get; set; } = Enumerable.Empty<Job>();
    }
}