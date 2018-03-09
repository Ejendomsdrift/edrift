using CategoryCore.Contract.Interfaces;

namespace Web.Models.Task
{
    public class AdHocViewModel : OperationalTaskViewModel
    {
        public ICategoryModel Category { get; set; }
        public string CategoryId { get; set; }       
    }
}