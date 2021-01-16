using System;
using System.Globalization;
using System.Numerics;
using AutoMapper;
using EthereumApi.Domain;
using EthereumApi.Infrastructure.RequestResponse;

namespace EthereumApi.Infrastructure.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<InfuraGetBlockApiResponse, Block>()
                .ForMember(dest => dest.Hash, opt => opt.MapFrom(src => src.Result.Hash))
                .ForMember(dest => dest.TransactionHashes, opt => opt.MapFrom(src => src.Result.Transactions))
                .ForMember(dest => dest.Number, opt => opt.MapFrom(src => Convert.ToUInt64(src.Result.Number, 16)));

            CreateMap<InfuraGetTransactionApiResponse, Transaction>()
                .ForMember(dest => dest.Hash, opt => opt.MapFrom(src => src.Result.Hash))
                .ForMember(dest => dest.BlockHash, opt => opt.MapFrom(src => src.Result.BlockHash))
                .ForMember(dest => dest.BlockNumber,
                    opt => opt.MapFrom(src => Convert.ToUInt64(src.Result.BlockNumber, 16)))
                .ForMember(dest => dest.Gas,
                    opt => opt.MapFrom(src =>
                        BigInteger.Parse(src.Result.Gas.Replace("0x", "0"), NumberStyles.AllowHexSpecifier)))
                .ForMember(dest => dest.Value,
                    opt => opt.MapFrom(src =>
                        BigInteger.Parse(src.Result.Value.Replace("0x", "0"), NumberStyles.AllowHexSpecifier)))
                .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.Result.From))
                .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.Result.To));
        }
    }
}