using System.Reflection;
using AutoMapper;
using EthereumApi.Core.Interfaces.Repositories;
using EthereumApi.Infrastructure.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace EthereumApi.Infrastructure
{
    public static class IocBootstrapper
    {
        public static void AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddScoped<IBlockRepository, BlockRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddSingleton<IMemoryCache>(_ => new MemoryCache(new MemoryCacheOptions()));
        }
    }
}