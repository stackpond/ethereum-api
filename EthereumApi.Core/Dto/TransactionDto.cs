namespace EthereumApi.Core.Dto
{
    public class TransactionDto
    {
        public string Hash { get; set; }
        public string BlockHash { get; set; }
        public ulong BlockNumber { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Value { get; set; }
        public string Gas { get; set; }
    }
}