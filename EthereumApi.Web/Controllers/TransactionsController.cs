using System.Threading.Tasks;
using EthereumApi.Core.Messaging;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("blocks/{blockNumber:int}/transactionCount")]
        public async Task<IActionResult> GetTransactionCountByBlockNumber(ulong blockNumber)
        {
            var commandResult = await _mediator.Send(new GetTransactionCountByBlockNumberCommand(blockNumber));
            return commandResult
                ? Ok(commandResult.Result)
                : StatusCode(500, new { commandResult.FailureReason });
        }

        [HttpGet("blocks/{blockNumber:int}/transactions")]
        public async Task<IActionResult> GetTransactionsByBlockNumber(ulong blockNumber, [FromQuery] int pageNumber)
        {
            var commandResult = await _mediator.Send(new GetTransactionsByBlockNumberAndAddressCommand(blockNumber, string.Empty, pageNumber));
            return commandResult
                ? Ok(commandResult.Result)
                : StatusCode(500, new { commandResult.FailureReason });
        }

        [HttpGet("transactions")]
        public async Task<IActionResult> GetTransactionsByBlockNumberAndAddress([FromQuery] ulong blockNumber, [FromQuery] string address, int pageNumber)
        {
            var commandResult = await _mediator.Send(new GetTransactionsByBlockNumberAndAddressCommand(blockNumber, address, pageNumber));
            return commandResult
                ? Ok(commandResult.Result)
                : StatusCode(500, new { commandResult.FailureReason });
        }
    }
}