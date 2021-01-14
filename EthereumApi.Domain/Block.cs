using System.Collections.Generic;

namespace EthereumApi.Domain
{
    public class Block
    {
        public ulong Number { get; set; }
        public string Hash { get; set; }
        public List<string> TransactionHashes { get; set; }
    }
}