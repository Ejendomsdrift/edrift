using System;
using System.Collections.Generic;
using Statistics.Contract.Interfaces.Models;

namespace Web.Models
{
    public class AddressInfoRequest : IAddressStatisticsCsvRequest
    {
        public string RangeDateString { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IEnumerable<AddressStatisticInfo> AddressStatisticInfos { get; set; }
    }
}