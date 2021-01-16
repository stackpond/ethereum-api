using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EthereumApi.Core.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EthereumApi.Core.Messaging
{
    public class GetTransactionCountByBlockNumberCommand : IRequest<CommandResult<uint>>
    {
        public GetTransactionCountByBlockNumberCommand(ulong blockNumber)
        {
            BlockNumber = blockNumber;
        }

        public ulong BlockNumber { get; }
    }

    public class GetTransactionCountByBlockNumberCommandHandler : IRequestHandler<
        GetTransactionCountByBlockNumberCommand,
        CommandResult<uint>>
    {
        private readonly ILogger<GetTransactionCountByBlockNumberCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly ITransactionRepository _transactionRepository;

        public GetTransactionCountByBlockNumberCommandHandler(IMapper mapper,
            ILogger<GetTransactionCountByBlockNumberCommandHandler> logger,
            ITransactionRepository transactionRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _transactionRepository = transactionRepository;
        }

        public async Task<CommandResult<uint>> Handle(GetTransactionCountByBlockNumberCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var blockTransactionCount =
                    await _transactionRepository.GetTransactionsCountByBlockNumber(request.BlockNumber);
                return new CommandResult<uint>(blockTransactionCount);
            }
            catch (Exception e)
            {
                var failureReason = string.Format(Resource.FailedToRetrieveTransactionsFormat,
                    request.BlockNumber);
                return new CommandResult<uint>(failureReason);
            }
        }
    }
}