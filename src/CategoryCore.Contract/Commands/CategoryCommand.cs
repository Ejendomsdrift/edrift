using System;
using Infrastructure.Messaging;

namespace CategoryCore.Contract.Commands
{
    public abstract class CategoryCommand : ICommand
    {
        public Guid Id { get; set; }

        protected CategoryCommand(Guid id)
        {
            Id = id;
        }
    }
}