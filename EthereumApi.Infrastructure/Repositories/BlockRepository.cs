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
    public class BlockRepository : IBlockRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public BlockRepository(IMapper mapper, IConfiguration configuration)
        {
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<Block> GetByNumber(ulong blockNumber)
        {
            var apiClient = new RestClient(_configuration[Constants.Infura.InfuraApiUrlConfigKey]);
            var apiRequest = new RestRequest(_configuration[Constants.Infura.InfuraProjectIdConfigKey], Method.POST);

            apiRequest.AddJsonBody(new
            {
                jsonrpc = Constants.Ethereum.JsonRpcVersion,
                method = Constants.Ethereum.GetBlockByNumberCommandName,
                @params = new JsonArray {$"0x{blockNumber:X2}", false},
                id = Constants.Ethereum.NetworkId
            }, Constants.JsonContentType);

            var apiResponse = await apiClient.ExecuteAsync(apiRequest);
            var apiBlockResponse = JsonConvert.DeserializeObject<InfuraGetBlockApiResponse>(apiResponse.Content);
            return _mapper.Map<Block>(apiBlockResponse);
        }
    }
}