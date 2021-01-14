namespace EthereumApi.Infrastructure
{
    public class Constants
    {
        public const string JsonContentType = "application/json";
        
        public class Infura
        {
            public const string InfuraApiUrlConfigKey = "infuraApiUrl";
            public const string InfuraProjectIdConfigKey = "infuraProjectId";
        }


        public class Ethereum
        {
            public const int NetworkId = 1;
            public const string JsonRpcVersion = "2.0";
            public const string GetBlockByNumberCommandName = "eth_getBlockByNumber";
            public const string GetTransactionHashCommandName = "eth_getTransactionByHash";
        }
    }
}