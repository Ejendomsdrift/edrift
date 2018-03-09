using System;

namespace CategoryCore.Contract.Commands
{
    public class UpdateCategory : CategoryCommand
    {
        public string Color { get; set; }
        public string Name { get; set; }

        public UpdateCategory(Guid id, string color, string name) : base(id)
        {
            Color = color;
            Name = name;
        }
    }
}