using System.Collections.Generic;

namespace EthereumApi.Infrastructure.RequestResponse
{
    public class InfuraGetBlockApiResponse
    {
        public InfuraGetBlockApiResponseResult Result { get; set; }
    }

    public class InfuraGetBlockApiResponseResult
    {
        public string Number { get; set; }
        public string Hash { get; set; }
        public List<string> Transactions { get; set; }
    }
}