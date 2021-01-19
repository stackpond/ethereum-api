using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EthereumApi.Core.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EthereumApi.Core.Messaging
{
    public class GetTransactionCountCommand : IRequest<CommandResult<uint>>
    {
        public GetTransactionCountCommand(ulong blockNumber)
        {
            BlockNumber = blockNumber;
        }

        public ulong BlockNumber { get; }
    }

    public class GetTransactionCountCommandCommandHandler : IRequestHandler<
        GetTransactionCountCommand,
        CommandResult<uint>>
    {
        private readonly ILogger<GetTransactionCountCommandCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly ITransactionRepository _transactionRepository;

        public GetTransactionCountCommandCommandHandler(IMapper mapper,
            ILogger<GetTransactionCountCommandCommandHandler> logger,
            ITransactionRepository transactionRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _transactionRepository = transactionRepository;
        }

        public async Task<CommandResult<uint>> Handle(GetTransactionCountCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var blockTransactionCount =
                    await _transactionRepository.GetTransactionsCount(request.BlockNumber);
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