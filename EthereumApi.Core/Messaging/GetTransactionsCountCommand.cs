using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EthereumApi.Core.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EthereumApi.Core.Messaging
{
    public class GetTransactionsCountCommand : IRequest<CommandResult<uint>>
    {
        public GetTransactionsCountCommand(ulong blockNumber)
        {
            BlockNumber = blockNumber;
        }

        public ulong BlockNumber { get; }
    }

    public class GetTransactionsCountCommandHandler : IRequestHandler<
        GetTransactionsCountCommand,
        CommandResult<uint>>
    {
        private readonly ILogger<GetTransactionsCountCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly ITransactionRepository _transactionRepository;

        public GetTransactionsCountCommandHandler(IMapper mapper,
            ILogger<GetTransactionsCountCommandHandler> logger,
            ITransactionRepository transactionRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _transactionRepository = transactionRepository;
        }

        public async Task<CommandResult<uint>> Handle(GetTransactionsCountCommand request,
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