using Infrastructure.EventSourcing.Implementation;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using YearlyPlanning.Contract.Models;

namespace YearlyPlanning.Contract.Events.JobEvents
{
    [BsonIgnoreExtraElements]
    public class JobAddressChanged : EventBase
    {
        public List<JobAddress> AddressList { get; set; } = new List<JobAddress>();
    }
}
