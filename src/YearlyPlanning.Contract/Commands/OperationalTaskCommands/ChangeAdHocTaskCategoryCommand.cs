using System;

namespace YearlyPlanning.Contract.Commands.OperationalTaskCommands
{
    public class ChangeAdHocTaskCategoryCommand : OperationalTaskCommand
    {
        public Guid CategoryId { get; set; }

        public ChangeAdHocTaskCategoryCommand(string id, Guid categoryId) : base(id)
        {
            CategoryId = categoryId;
        }
    }
}
