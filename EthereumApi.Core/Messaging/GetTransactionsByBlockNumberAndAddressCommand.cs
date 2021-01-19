using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EthereumApi.Core.Dto;
using EthereumApi.Core.Interfaces.Repositories;
using EthereumApi.Domain;
using MediatR;
using Microsoft.Extensions.Logging;
using TransactionDtoCollection = System.Collections.Generic.List<EthereumApi.Core.Dto.TransactionDto>;

namespace EthereumApi.Core.Messaging
{
    public class GetTransactionsByBlockNumberAndAddressCommand : IRequest<CommandResult<TransactionDtoCollection>>
    {
        public GetTransactionsByBlockNumberAndAddressCommand(ulong blockNumber, string address, int pageNumber)
        {
            BlockNumber = blockNumber;
            Address = address;
            PageNumber = pageNumber;
        }

        public ulong BlockNumber { get; }
        public string Address { get; }
        public int PageNumber { get; }
    }

    public class GetTransactionsByBlockNumberAndAddressCommandHandler : IRequestHandler<GetTransactionsByBlockNumberAndAddressCommand,
        CommandResult<TransactionDtoCollection>>
    {
        private readonly ILogger<GetTransactionsByBlockNumberAndAddressCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly ITransactionRepository _transactionRepository;

        public GetTransactionsByBlockNumberAndAddressCommandHandler(IMapper mapper,
            ILogger<GetTransactionsByBlockNumberAndAddressCommandHandler> logger, ITransactionRepository transactionRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _transactionRepository = transactionRepository;
        }

        public async Task<CommandResult<TransactionDtoCollection>> Handle(GetTransactionsByBlockNumberAndAddressCommand request,
            CancellationToken cancellationToken)
        {
            string failureReason;
            var failureException = default(Exception);
            var transactions = default(IEnumerable<Transaction>);
            try
            {
                if (string.IsNullOrWhiteSpace(request.Address))
                {
                    transactions = await _transactionRepository.GetTransactionsByBlockNumber(request.BlockNumber, request.PageNumber);
                }
                else
                {
                    transactions = await _transactionRepository.GetTransactionsByBlockNumberAndAddress(request.BlockNumber, request.Address, request.PageNumber);
                }

                var transactionDtos = _mapper.Map<IEnumerable<TransactionDto>>(transactions);

                if (transactionDtos.Any())
                    return new CommandResult<TransactionDtoCollection>(new TransactionDtoCollection(transactionDtos));

                failureReason = string.Format(Resource.TransactionsNotFoundFormat,
                    request.Address);
            }
            catch (Exception e)
            {
                failureException = e;
                failureReason = string.Format(Resource.FailedToRetrieveTransactionsFormat,
                    request.Address);
            }

            _logger.LogError(failureException, failureReason);
            return new CommandResult<TransactionDtoCollection>(failureReason);
        }
    }
}