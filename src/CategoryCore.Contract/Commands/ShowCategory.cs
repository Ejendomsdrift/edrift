using System;

namespace CategoryCore.Contract.Commands
{
    public class ShowCategory : CategoryCommand
    {
        public ShowCategory(Guid id) : base(id)
        {
        }
    }
}