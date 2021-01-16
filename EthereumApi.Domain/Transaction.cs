using System.Numerics;

namespace EthereumApi.Domain
{
    public class Transaction
    {
        public string Hash { get; set; }
        public string BlockHash { get; set; }
        public ulong BlockNumber { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public BigInteger Value { get; set; }
        public BigInteger Gas { get; set; }
    }
}