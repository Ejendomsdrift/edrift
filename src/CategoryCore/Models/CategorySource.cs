using System;
using CategoryCore.Contract.Events;
using Infrastructure.EventSourcing.Implementation;

namespace CategoryCore.Models
{
    public class CategorySource : AggregateBase
    {
        public Guid? ParentId { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public bool Visible { get; set; }

        public CategorySource()
        {
            RegisterTransition<CategoryCreated>(Apply);
            RegisterTransition<CategoryNameSet>(Apply);
            RegisterTransition<CategoryColorSet>(Apply);
            RegisterTransition<CategoryHidden>(Apply);
            RegisterTransition<CategoryShown>(Apply);
        }

        public CategorySource(Guid id, Guid? parentId, string name, string color, bool visible) : this()
        {
            Id = id.ToString();

            RaiseEvent(new CategoryCreated
            {
                ParentId = parentId,
                Name = name,
                Color = color,
                Visible = visible,
            });
        }

        private void Apply(CategoryCreated e)
        {
            Id = e.SourceId;
            ParentId = e.ParentId;
            Name = e.Name;
            Color = e.Color;
            Visible = e.Visible;
        }

        public void SetName(string newName)
        {
            RaiseEvent(new CategoryNameSet { Name = newName });
        }

        private void Apply(CategoryNameSet e)
        {
            Name = e.Name;
        }

        public void SetColor(string newColor)
        {
            RaiseEvent(new CategoryColorSet { Color = newColor });
        }

        private void Apply(CategoryColorSet e)
        {
            Color = e.Color;
        }

        public void Hide()
        {
            RaiseEvent(new CategoryHidden());
        }

        private void Apply(CategoryHidden e)
        {
            Visible = false;
        }

        public void Show()
        {
            RaiseEvent(new CategoryShown());
        }

        private void Apply(CategoryShown e)
        {
            Visible = true;
        }

        public static CategorySource Create(Guid id, Guid? parentId, string name, string color, bool visible)
        {
            return new CategorySource(id, parentId, name, color, visible);
        }
    }
}
