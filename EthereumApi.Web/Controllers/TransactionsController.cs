using System.Threading.Tasks;
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
        public async Task<IActionResult> GetByAddress(string address, [FromQuery] int pageNumber)
        {
            var commandResult = await _mediator.Send(new GetTransactionsByAddressCommand(address, pageNumber));
            return commandResult
                ? Ok(commandResult.Result)
                : StatusCode(500, new {commandResult.FailureReason});
        }


        [HttpGet("blocks/{blockNumber:int}/transactions")]
        public async Task<IActionResult> GetByBlockNumber(ulong blockNumber, [FromQuery] int pageNumber)
        {
            var commandResult = await _mediator.Send(new GetTransactionsByBlockNumberCommand(blockNumber, pageNumber));
            return commandResult
                ? Ok(commandResult.Result)
                : StatusCode(500, new {commandResult.FailureReason});
        }

        [HttpGet("blocks/{blockNumber:int}/transactionCount")]
        public async Task<IActionResult> GetTransactionCountByBlockNumber(ulong blockNumber)
        {
            var commandResult = await _mediator.Send(new GetTransactionCountByBlockNumberCommand(blockNumber));
            return commandResult
                ? Ok(commandResult.Result)
                : StatusCode(500, JsonConvert.SerializeObject(new {commandResult.FailureReason}));
        }
    }
}