using System;
using System.Collections.Generic;
using MongoRepository.Contract.Interfaces;

namespace Groups.Models
{
    public class Group: IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid ManagementId { get; set; }
        public bool Deleted { get; set; }
        public List<Guid> MemberIds { get; set; } = new List<Guid>();
    }
}
