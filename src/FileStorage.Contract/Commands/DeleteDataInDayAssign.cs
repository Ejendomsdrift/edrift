using System;

namespace FileStorage.Contract.Commands
{
    public class DeleteDataInDayAssign : FileCommand
    {
        public DeleteDataInDayAssign(Guid id) : base(id.ToString())
        {
        }
    }
}
