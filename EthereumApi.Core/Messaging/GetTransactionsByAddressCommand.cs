﻿using System;
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
    public class GetTransactionsByAddressCommand : IRequest<CommandResult<TransactionDtoCollection>>
    {
        public GetTransactionsByAddressCommand(string address, int pageNumber)
        {
            Address = address;
            PageNumber = pageNumber;
        }

        public string Address { get; }
        public int PageNumber { get; }
    }

    public class GetTransactionsByAddressCommandHandler : IRequestHandler<GetTransactionsByAddressCommand,
        CommandResult<TransactionDtoCollection>>
    {
        private readonly ILogger<GetTransactionsByAddressCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly ITransactionRepository _transactionRepository;

        public GetTransactionsByAddressCommandHandler(IMapper mapper,
            ILogger<GetTransactionsByAddressCommandHandler> logger, ITransactionRepository transactionRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _transactionRepository = transactionRepository;
        }

        public async Task<CommandResult<TransactionDtoCollection>> Handle(GetTransactionsByAddressCommand request,
            CancellationToken cancellationToken)
        {
            string failureReason;
            var failureException = default(Exception);

            try
            {
                var transactions =
                    await _transactionRepository.GetTransactionsByAddress(request.Address, request.PageNumber);
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