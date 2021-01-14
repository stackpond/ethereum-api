using System.Collections.Generic;
using System.Threading.Tasks;
using EthereumApi.Core.Dto;
using EthereumApi.Core.Messaging;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EthereumApi.Web.Controllers
{
    [ApiController]
    [Route("api")]
    public class TransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TransactionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("addresses/{address}/transactions")]
        public IEnumerable<TransactionDto> GetByAddress(string address)
        {
            return new List<TransactionDto>();
        }


        [HttpGet("blocks/{blockNumber:int}/transactions")]
        public async Task<IActionResult> GetByBlockNumber(ulong blockNumber)
        {
            var commandResult = await _mediator.Send(new GetTransactionsByBlockNumberCommand(blockNumber));
            return commandResult
                ? Ok(commandResult.Result)
                : StatusCode(500, JsonConvert.SerializeObject(new {commandResult.FailureReason}));
        }
    }
}