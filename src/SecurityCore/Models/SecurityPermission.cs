using MongoRepository.Contract.Interfaces;
using System;
using System.Collections.Generic;

namespace SecurityCore.Models
{
    public class SecurityPermission : IEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Key { get; set; }
        public string GroupName { get; set; }
        public List<Rule> Rules { get; set; } = new List<Rule>();
    }
}
