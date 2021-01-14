using System.Collections.Generic;

namespace EthereumApi.Core.Dto
{
    public class TransactionDto
    {
        public string Hash { get; set; }
        public string BlockHash { get; set; }
        public ulong BlockNumber { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public ulong Value { get; set; }
        public double Gas { get; set; }
    }
}