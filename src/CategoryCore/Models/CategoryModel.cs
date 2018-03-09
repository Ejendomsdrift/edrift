using System;
using System.Collections.Generic;
using CategoryCore.Contract.Interfaces;

namespace CategoryCore.Models
{
    public class CategoryModel : ICategoryModel
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public string Color { get; set; }
        public string Name { get; set; }
        public bool Visible { get; set; }
        public ICategoryModel Parent { get; set; }
        public List<ICategoryModel> Children { get; set; } = new List<ICategoryModel>();
    }
}
