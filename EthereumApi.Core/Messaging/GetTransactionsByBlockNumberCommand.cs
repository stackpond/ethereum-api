using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EthereumApi.Core.Dto;
using EthereumApi.Core.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

using TransactionDtoCollection = System.Collections.Generic.List<EthereumApi.Core.Dto.TransactionDto>;
namespace EthereumApi.Core.Messaging
{
    public class GetTransactionsByBlockNumberCommand : IRequest<CommandResult<TransactionDtoCollection>>
    {
        public GetTransactionsByBlockNumberCommand(ulong blockNumber)
        {
            BlockNumber = blockNumber;
        }

        public ulong BlockNumber { get; }
    }

    public class GetTransactionsByBlockNumberCommandHandler : IRequestHandler<GetTransactionsByBlockNumberCommand,
        CommandResult<TransactionDtoCollection>>
    {
        private readonly ILogger<GetTransactionsByBlockNumberCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly ITransactionRepository _transactionRepository;

        public GetTransactionsByBlockNumberCommandHandler(IMapper mapper,
            ILogger<GetTransactionsByBlockNumberCommandHandler> logger, ITransactionRepository transactionRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _transactionRepository = transactionRepository;
        }

        public async Task<CommandResult<TransactionDtoCollection>> Handle(GetTransactionsByBlockNumberCommand request,
            CancellationToken cancellationToken)
        {
            string failureReason;
            var failureException = default(Exception);

            try
            {
                var transactions = await _transactionRepository.GetTransactionsByBlockNumber(request.BlockNumber);
                var transactionDtos = _mapper.Map<IEnumerable<TransactionDto>>(transactions);

                if (transactionDtos.Any())
                    return new CommandResult<TransactionDtoCollection>(new TransactionDtoCollection(transactionDtos));

                failureReason = string.Format(Resource.TransactionsNotFoundFormat,
                    request.BlockNumber);
            }
            catch (Exception e)
            {
                failureException = e;
                failureReason = string.Format(Resource.FailedToRetrieveTransactionsFormat,
                    request.BlockNumber);
            }

            _logger.LogError(failureException, failureReason);
            return new CommandResult<TransactionDtoCollection>(failureReason);
        }
    }
}