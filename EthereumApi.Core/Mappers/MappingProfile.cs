using AutoMapper;
using EthereumApi.Core.Dto;
using EthereumApi.Domain;

namespace EthereumApi.Core.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Transaction, TransactionDto>();
        }
    }
}