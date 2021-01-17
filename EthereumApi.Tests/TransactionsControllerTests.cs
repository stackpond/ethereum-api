using EthereumApi.Core.Messaging;
using EthereumApi.Web.Controllers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using TransactionDtoCollection = System.Collections.Generic.List<EthereumApi.Core.Dto.TransactionDto>;

namespace EthereumApi.Tests
{
    public class TransactionsControllerTests
    {
        private readonly TransactionsController _transactionsController;
        private readonly Mock<IMediator> _mediator = new Mock<IMediator>();

        public TransactionsControllerTests()
        {
            _transactionsController = new TransactionsController(_mediator.Object);
        }

        [Test]
        public async Task ShouldReturn200WhenGetByBlockNumberSucceeds()
        {
            // Arrange
            _mediator.Setup(m => m.Send(It.IsAny<GetTransactionsByBlockNumberCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CommandResult<TransactionDtoCollection>(new TransactionDtoCollection()));

            // Act
            var actionResult = await _transactionsController.GetByBlockNumber(1, 1) as ObjectResult;

            // Assert
            Assert.NotNull(actionResult);
            Assert.AreEqual(StatusCodes.Status200OK, actionResult.StatusCode.Value);
        }

        [Test]
        public async Task ShouldReturn500WhenGetByBlockNumberFails()
        {
            // Arrange
            _mediator.Setup(m => m.Send(It.IsAny<GetTransactionsByBlockNumberCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CommandResult<TransactionDtoCollection>("Failed"));

            // Act
            var actionResult = await _transactionsController.GetByBlockNumber(1, 1) as ObjectResult;

            // Assert
            Assert.NotNull(actionResult);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, actionResult.StatusCode.Value);
        }

        [Test]
        public async Task ShouldReturn200WhenGetByAddressSucceeds()
        {
            // Arrange
            _mediator.Setup(m => m.Send(It.IsAny<GetTransactionsByAddressCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CommandResult<TransactionDtoCollection>(new TransactionDtoCollection()));

            // Act
            var actionResult = await _transactionsController.GetByAddress("0x22a52e0f761345ecffba06482c99dc3dea7bd8f1", 1) as ObjectResult;

            // Assert
            Assert.NotNull(actionResult);
            Assert.AreEqual(StatusCodes.Status200OK, actionResult.StatusCode.Value);
        }

        [Test]
        public async Task ShouldReturn500WhenGetByAddressFails()
        {
            // Arrange
            _mediator.Setup(m => m.Send(It.IsAny<GetTransactionsByAddressCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CommandResult<TransactionDtoCollection>("Failed"));

            // Act
            var actionResult = await _transactionsController.GetByAddress("0x22a52e0f761345ecffba06482c99dc3dea7bd8f1", 1) as ObjectResult;

            // Assert
            Assert.NotNull(actionResult);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, actionResult.StatusCode.Value);
        }
    }
}