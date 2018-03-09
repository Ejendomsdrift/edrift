using System;

namespace CategoryCore.Contract.Commands
{
    public class HideCategory : CategoryCommand
    {
        public HideCategory(Guid id) : base(id)
        {
        }
    }
}