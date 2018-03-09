using System;
using Infrastructure.EventSourcing.Implementation;

namespace CategoryCore.Contract.Events
{
    public class CategoryCreated : EventBase
    {
        public Guid? ParentId { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public bool Visible { get; set; }
    }
}