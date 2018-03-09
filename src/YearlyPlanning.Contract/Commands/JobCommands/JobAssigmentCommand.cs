namespace YearlyPlanning.Contract.Commands.JobCommands
{
    public abstract class JobAssigmentCommand : JobCommand
    {
        public string DepartmentId { get; set; }

        protected JobAssigmentCommand(string id, string departmentId) : base(id)
        {
            DepartmentId = departmentId;
        }
    }
}