using System;
using MongoRepository.Contract.Interfaces;

namespace CategoryCore.Models
{
    public class Category : IEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? ParentId { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public bool Visible { get; set; }
    }
}
