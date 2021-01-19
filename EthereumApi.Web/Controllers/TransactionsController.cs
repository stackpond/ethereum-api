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

        [HttpGet("transactionsCount")]
        public async Task<IActionResult> GetTransactionsCount([FromQuery] ulong blockNumber)
        {
            var commandResult = await _mediator.Send(new GetTransactionsCountCommand(blockNumber));
            return commandResult
                ? Ok(commandResult.Result)
                : StatusCode(500, new { commandResult.FailureReason });
        }

        [HttpGet("transactions")]
        public async Task<IActionResult> GetTransactions([FromQuery] ulong blockNumber, [FromQuery] string address, int pageNumber)
        {
            var commandResult = await _mediator.Send(new GetTransactionsCommand(blockNumber, address, pageNumber));
            return commandResult
                ? Ok(commandResult.Result)
                : StatusCode(500, new { commandResult.FailureReason });
        }
    }
}