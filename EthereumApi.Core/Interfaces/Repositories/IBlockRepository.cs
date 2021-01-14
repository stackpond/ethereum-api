using System.Threading.Tasks;
using EthereumApi.Domain;

namespace EthereumApi.Core.Interfaces.Repositories
{
    public interface IBlockRepository
    {
        Task<Block> GetByNumber(ulong blockNumber);
    }
}