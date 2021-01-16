using System;
using System.Threading.Tasks;
using AutoMapper;
using EthereumApi.Core.Interfaces.Repositories;
using EthereumApi.Domain;
using EthereumApi.Infrastructure.RequestResponse;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;

namespace EthereumApi.Infrastructure.Repositories
{
    public class BlockRepository : IBlockRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;

        public BlockRepository(IMapper mapper, IConfiguration configuration, IMemoryCache memoryCache)
        {
            _mapper = mapper;
            _configuration = configuration;
            _memoryCache = memoryCache;
        }

        public async Task<Block> GetLatest()
        {
            var apiClient = new RestClient(_configuration[Constants.Infura.InfuraApiUrlConfigKey]);
            var apiRequest =
                new RestRequest(_configuration[Constants.Infura.InfuraProjectIdConfigKey], Method.POST);

            apiRequest.AddJsonBody(new
            {
                jsonrpc = Constants.Ethereum.JsonRpcVersion,
                method = Constants.Ethereum.GetLatestBlockNumberCommandName,
                @params = new JsonArray(),
                id = Constants.Ethereum.NetworkId
            }, Constants.JsonContentType);

            var apiResponse = await apiClient.ExecuteAsync(apiRequest);
            var apiBlockResponse =
                JsonConvert.DeserializeObject<InfuraGetLatestBlockRequestResponse>(apiResponse.Content);
            var latestBlockNumber = Convert.ToUInt64(apiBlockResponse.Result, 16);
            return await GetByNumber(latestBlockNumber);
        }

        public async Task<Block> GetByNumber(ulong blockNumber)
        {
            var cacheKey = $"Block_{blockNumber}";

            if (!_memoryCache.TryGetValue(cacheKey, out Block block))
            {
                var apiClient = new RestClient(_configuration[Constants.Infura.InfuraApiUrlConfigKey]);
                var apiRequest =
                    new RestRequest(_configuration[Constants.Infura.InfuraProjectIdConfigKey], Method.POST);

                apiRequest.AddJsonBody(new
                {
                    jsonrpc = Constants.Ethereum.JsonRpcVersion,
                    method = Constants.Ethereum.GetBlockByNumberCommandName,
                    @params = new JsonArray {$"0x{blockNumber:X2}", false},
                    id = Constants.Ethereum.NetworkId
                }, Constants.JsonContentType);

                var apiResponse = await apiClient.ExecuteAsync(apiRequest);
                var apiBlockResponse = JsonConvert.DeserializeObject<InfuraGetBlockApiResponse>(apiResponse.Content);
                block = _mapper.Map<Block>(apiBlockResponse);

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSize(1)
                    .SetSlidingExpiration(TimeSpan.FromMinutes(int.Parse(_configuration["cacheExpirationInMinutes"])));
                _memoryCache.Set(cacheKey, block, cacheEntryOptions);
            }

            return block;
        }
    }
}