using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EthereumApi.Core.Interfaces.Repositories;
using EthereumApi.Domain;
using EthereumApi.Infrastructure.RequestResponse;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;

namespace EthereumApi.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly IBlockRepository _blockRepository;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;

        public TransactionRepository(IMapper mapper, IConfiguration configuration, IBlockRepository blockRepository,
            IMemoryCache memoryCache, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _configuration = configuration;
            _blockRepository = blockRepository;
            _memoryCache = memoryCache;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<uint> GetTransactionsCount(ulong blockNumber)
        {
            var cacheKey = $"BlockTransactionCount_{blockNumber}";

            if (!_memoryCache.TryGetValue(cacheKey, out uint blockTransactionCount))
            {
                var apiClient = new RestClient(_configuration[Constants.Infura.InfuraApiUrlConfigKey]);
                var apiRequest =
                    new RestRequest(_configuration[Constants.Infura.InfuraProjectIdConfigKey], Method.POST);

                apiRequest.AddJsonBody(new
                {
                    jsonrpc = Constants.Ethereum.JsonRpcVersion,
                    method = Constants.Ethereum.GetBlockTransactionCountCommandName,
                    @params = new JsonArray { $"0x{blockNumber:X2}" },
                    id = Constants.Ethereum.NetworkId
                }, Constants.JsonContentType);

                var apiResponse = await apiClient.ExecuteAsync(apiRequest);
                var apiBlockTransactionCountResponse =
                    JsonConvert.DeserializeObject<InfuraGetBlockTransactionCountApiResponse>(apiResponse.Content);
                blockTransactionCount = Convert.ToUInt32(apiBlockTransactionCountResponse.Result, 16);
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSize(1)
                    .SetSlidingExpiration(TimeSpan.FromMinutes(int.Parse(_configuration["cacheExpirationInMinutes"])));
                _memoryCache.Set(cacheKey, blockTransactionCount, cacheEntryOptions);
            }

            return blockTransactionCount;
        }

        public async Task<IEnumerable<Transaction>> GetTransactions(ulong blockNumber, int pageNumber)
        {
            var transactions = new List<Transaction>();
            var block = await _blockRepository.GetByNumber(blockNumber);
            var apiClient = new RestClient(_configuration[Constants.Infura.InfuraApiUrlConfigKey]);

            var numberOfItemsPerPage = int.Parse(_configuration["numberOfItemsPerPage"]);

            foreach (var transactionHash in block.TransactionHashes.Skip(numberOfItemsPerPage * (pageNumber - 1))
                .Take(numberOfItemsPerPage))
            {
                var cacheKey = $"Transaction_{transactionHash}";
                _httpContextAccessor.HttpContext.RequestAborted.ThrowIfCancellationRequested();

                if (!_memoryCache.TryGetValue(cacheKey, out Transaction transaction))
                {
                    var apiRequest =
                        new RestRequest(_configuration[Constants.Infura.InfuraProjectIdConfigKey], Method.POST);

                    apiRequest.AddJsonBody(new
                    {
                        jsonrpc = Constants.Ethereum.JsonRpcVersion,
                        method = Constants.Ethereum.GetTransactionHashCommandName,
                        @params = new JsonArray { transactionHash },
                        id = Constants.Ethereum.NetworkId
                    }, Constants.JsonContentType);

                    var apiResponse = await apiClient.ExecuteAsync(apiRequest);
                    var apiTransactionResponse =
                        JsonConvert.DeserializeObject<InfuraGetTransactionApiResponse>(apiResponse.Content);

                    transaction = _mapper.Map<Transaction>(apiTransactionResponse);

                    var cacheEntryOptions = new MemoryCacheEntryOptions().SetSize(1)
                        .SetSlidingExpiration(
                            TimeSpan.FromMinutes(int.Parse(_configuration["cacheExpirationInMinutes"])));
                    _memoryCache.Set(cacheKey, transaction, cacheEntryOptions);
                }

                transactions.Add(transaction);
            }

            return transactions;
        }

        public async Task<IEnumerable<Transaction>> GetTransactions(ulong blockNumber, string address, int pageNumber)
        {
            var transactions = new List<Transaction>();
            var block = await _blockRepository.GetByNumber(blockNumber);
            var apiClient = new RestClient(_configuration[Constants.Infura.InfuraApiUrlConfigKey]);

            var numberOfItemsPerPage = int.Parse(_configuration["numberOfItemsPerPage"]);
            var minimumTransactionsToProcess = numberOfItemsPerPage * pageNumber;

            foreach (var transactionHash in block.TransactionHashes)
            {
                var cacheKey = $"Transaction_{transactionHash}";
                _httpContextAccessor.HttpContext.RequestAborted.ThrowIfCancellationRequested();

                if (!_memoryCache.TryGetValue(cacheKey, out Transaction transaction))
                {
                    var apiRequest =
                        new RestRequest(_configuration[Constants.Infura.InfuraProjectIdConfigKey], Method.POST);

                    apiRequest.AddJsonBody(new
                    {
                        jsonrpc = Constants.Ethereum.JsonRpcVersion,
                        method = Constants.Ethereum.GetTransactionHashCommandName,
                        @params = new JsonArray { transactionHash },
                        id = Constants.Ethereum.NetworkId
                    }, Constants.JsonContentType);

                    var apiResponse = await apiClient.ExecuteAsync(apiRequest);
                    var apiTransactionResponse =
                        JsonConvert.DeserializeObject<InfuraGetTransactionApiResponse>(apiResponse.Content);

                    transaction = _mapper.Map<Transaction>(apiTransactionResponse);

                    var cacheEntryOptions = new MemoryCacheEntryOptions().SetSize(1)
                        .SetSlidingExpiration(
                            TimeSpan.FromMinutes(int.Parse(_configuration["cacheExpirationInMinutes"])));
                    _memoryCache.Set(cacheKey, transaction, cacheEntryOptions);
                }

                if (!string.IsNullOrWhiteSpace(transaction.To) && transaction.To.Equals(address, StringComparison.OrdinalIgnoreCase) ||
                    !string.IsNullOrWhiteSpace(transaction.From) && transaction.From.Equals(address, StringComparison.OrdinalIgnoreCase))
                {
                    transactions.Add(transaction);
                }

                if (transactions.Count == minimumTransactionsToProcess)
                    return transactions.Skip(numberOfItemsPerPage * (pageNumber - 1)).Take(numberOfItemsPerPage);
            }

            return transactions;
        }
    }
}