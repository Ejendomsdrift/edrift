using System;
using System.Collections.Generic;
using MemberCore.Contract.Enums;
using MongoDB.Bson.Serialization.Attributes;
using MongoRepository.Contract.Interfaces;

namespace MemberCore.Models
{
    [BsonIgnoreExtraElements]
    public class Member : IEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string UserName { get; set; }

        public List<Role> RoleList { get; set; } = new List<Role>();

        public RoleType? CurrentRole { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string MobilePhone { get; set; }

        public string WorkingPhone { get; set; }

        public bool HasAvatar { get; set; }

        public bool IsDeleted { get; set; }

        public int? DaysAhead { get; set; }
    }
}
