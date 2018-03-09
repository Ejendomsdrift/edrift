using AutoMapper;
using Infrastructure.Interfaces;
using Statistics.Contract.Interfaces.Models;

namespace Statistics.Core.Profiles
{
    public class AddressStatisticInfoProfile : Profile, IMapProfile
    {
        public AddressStatisticInfoProfile()
        {
            CreateMap<IAddressStatisticInfo, AddressStatisticInfo>();
        }
    }
}
