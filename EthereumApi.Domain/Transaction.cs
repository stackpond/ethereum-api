namespace EthereumApi.Domain
{
    public class Transaction
    {
        public string Hash { get; set; }
        public string BlockHash { get; set; }
        public ulong BlockNumber { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public ulong Value { get; set; }
        public ulong Gas { get; set; }
    }
}