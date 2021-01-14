using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EthereumApi.Core.Interfaces.Repositories;
using EthereumApi.Domain;
using EthereumApi.Infrastructure.RequestResponse;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;

namespace EthereumApi.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly IBlockRepository _blockRepository;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public TransactionRepository(IMapper mapper, IConfiguration configuration, IBlockRepository blockRepository)
        {
            _mapper = mapper;
            _configuration = configuration;
            _blockRepository = blockRepository;
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByAddress(ulong blockNumber)
        {
            return await Task.Run(() => new List<Transaction>());
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByBlockNumber(ulong blockNumber)
        {
            var transactions = new List<Transaction>();
            var block = await _blockRepository.GetByNumber(blockNumber);
            var apiClient = new RestClient(_configuration[Constants.Infura.InfuraApiUrlConfigKey]);

            foreach (var transactionHash in block.TransactionHashes)
            {
                var apiRequest =
                    new RestRequest(_configuration[Constants.Infura.InfuraProjectIdConfigKey], Method.POST);

                apiRequest.AddJsonBody(new
                {
                    jsonrpc = Constants.Ethereum.JsonRpcVersion,
                    method = Constants.Ethereum.GetTransactionHashCommandName,
                    @params = new JsonArray {transactionHash},
                    id = Constants.Ethereum.NetworkId
                }, Constants.JsonContentType);

                var apiResponse = await apiClient.ExecuteAsync(apiRequest);
                var apiTransactionResponse =
                    JsonConvert.DeserializeObject<InfuraGetTransactionApiResponse>(apiResponse.Content);

                var transaction = _mapper.Map<Transaction>(apiTransactionResponse);
                transactions.Add(transaction);
            }

            return transactions;
        }
    }
}