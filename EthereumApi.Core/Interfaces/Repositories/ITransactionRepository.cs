using System.Collections.Generic;
using System.Threading.Tasks;
using EthereumApi.Domain;

namespace EthereumApi.Core.Interfaces.Repositories
{
    public interface ITransactionRepository
    {
        Task<uint> GetTransactionsCountByBlockNumber(ulong blockNumber);
        Task<IEnumerable<Transaction>> GetTransactionsByAddress(string address, int pageNumber);
        Task<IEnumerable<Transaction>> GetTransactionsByBlockNumber(ulong blockNumber, int pageNumber);
        Task<Transaction> GetTransactionByBlockNumberAndIndex(ulong blockNumber, int index);
    }
}