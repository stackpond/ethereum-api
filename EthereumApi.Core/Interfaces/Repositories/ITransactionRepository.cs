using System.Collections.Generic;
using System.Threading.Tasks;
using EthereumApi.Domain;

namespace EthereumApi.Core.Interfaces.Repositories
{
    public interface ITransactionRepository
    {
        Task<IEnumerable<Transaction>> GetTransactionsByAddress(ulong blockNumber);
        Task<IEnumerable<Transaction>> GetTransactionsByBlockNumber(ulong blockNumber);
    }
}