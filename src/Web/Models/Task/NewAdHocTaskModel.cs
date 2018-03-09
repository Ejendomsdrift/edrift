using System;
using CategoryCore.Contract.Interfaces;

namespace Web.Models.Task
{
    public class NewAdHocTaskModel : NewOperationalTaskModel
    {
        public ICategoryModel Category { get; set; }
        public Guid CategoryId { get; set; }
    }
}