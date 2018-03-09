using System;

namespace FileStorage.Contract.Commands
{
    public class Delete : FileCommand
    {
        public Delete(Guid id) : base(id.ToString())
        {
        }        
    }
}