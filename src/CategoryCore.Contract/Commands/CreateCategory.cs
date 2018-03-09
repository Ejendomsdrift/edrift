using System;

namespace CategoryCore.Contract.Commands
{
    public class CreateCategory : CategoryCommand
    {
        public Guid? ParentId { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public bool Visible { get; set; }

        public CreateCategory(Guid id, Guid? parentId, string name, string color, bool visible) : base(id)
        {
            ParentId = parentId;
            Name = name;
            Color = color;
            Visible = visible;
        }
    }
}