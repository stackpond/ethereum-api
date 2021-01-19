using System.Collections.Generic;
using System.Threading.Tasks;
using EthereumApi.Domain;

namespace EthereumApi.Core.Interfaces.Repositories
{
    public interface ITransactionRepository
    {
        Task<uint> GetTransactionsCount(ulong blockNumber);
        Task<IEnumerable<Transaction>> GetTransactions(ulong blockNumber, int pageNumber);
        Task<IEnumerable<Transaction>> GetTransactions(ulong blockNumber, string address, int pageNumber);
    }
}