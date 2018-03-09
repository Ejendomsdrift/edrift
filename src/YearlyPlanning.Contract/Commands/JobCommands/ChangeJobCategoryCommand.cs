using System;

namespace YearlyPlanning.Contract.Commands.JobCommands
{
    public class ChangeJobCategoryCommand : JobCommand
    {
        public Guid CategoryId { get; set; }

        public ChangeJobCategoryCommand(string id, Guid categoryId) : base(id)
        {
            CategoryId = categoryId;
        }
    }
}