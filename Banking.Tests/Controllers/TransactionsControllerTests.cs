using Banking.Controllers;
using Banking.DTO;
using Banking.Entity;
using Banking.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Banking.Tests.Controllers
{
    public class TransactionsControllerTests
    {
        private readonly Mock<ITransactionRepository> _mockTransactionRepository;
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly TransactionsController _controller;

        public TransactionsControllerTests()
        {
            _mockTransactionRepository = new Mock<ITransactionRepository>();
            
            var mockUserStore = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(
                mockUserStore.Object, null, null, null, null, null, null, null, null);

            _controller = new TransactionsController(_mockTransactionRepository.Object, _mockUserManager.Object);
        }

        [Fact]
        public async Task Transfer_ValidTransfer_ReturnsOkResult()
        {
            var transferDto = new TransferDto
            {
                FromAccountNumber = "12345",
                ToAccountNumber = "67890",
                Amount = 100
            };

            _mockTransactionRepository.Setup(repo => repo.TransferAsync(transferDto))
                .ReturnsAsync(true);

            var result = await _controller.Transfer(transferDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value;
            Assert.NotNull(response);
        }

        [Fact]
        public async Task Transfer_InvalidTransfer_ReturnsBadRequest()
        {
            var transferDto = new TransferDto
            {
                FromAccountNumber = "12345",
                ToAccountNumber = "67890",
                Amount = 100
            };

            _mockTransactionRepository.Setup(repo => repo.TransferAsync(transferDto))
                .ReturnsAsync(false);

            var result = await _controller.Transfer(transferDto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Transfer failed. Please check account numbers and balance.", badRequestResult.Value);
        }

        [Fact]
        public async Task Deposit_ValidDeposit_ReturnsOkResult()
        {
            var depositDto = new DepositDto
            {
                AccountNumber = "12345",
                Amount = 100
            };

            _mockTransactionRepository.Setup(repo => repo.DepositAsync(depositDto))
                .ReturnsAsync(true);

            var result = await _controller.Deposit(depositDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value;
            Assert.NotNull(response);
        }

        [Fact]
        public async Task Withdraw_ValidWithdraw_ReturnsOkResult()
        {
            var withdrawDto = new WithdrawDto
            {
                AccountNumber = "12345",
                Amount = 50,
                Password = "password123"
            };

            var user = new User { Id = "user1", Email = "test@example.com" };

            _mockTransactionRepository.Setup(repo => repo.GetUserByAccountNumberAsync(withdrawDto.AccountNumber))
                .ReturnsAsync(user);
            _mockUserManager.Setup(um => um.CheckPasswordAsync(user, withdrawDto.Password))
                .ReturnsAsync(true);
            _mockTransactionRepository.Setup(repo => repo.WithdrawAsync(withdrawDto))
                .ReturnsAsync(true);

            var result = await _controller.Withdraw(withdrawDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value;
            Assert.NotNull(response);
        }

        [Fact]
        public async Task Withdraw_UserNotFound_ReturnsNotFound()
        {
            var withdrawDto = new WithdrawDto
            {
                AccountNumber = "12345",
                Amount = 50,
                Password = "password123"
            };

            _mockTransactionRepository.Setup(repo => repo.GetUserByAccountNumberAsync(withdrawDto.AccountNumber))
                .ReturnsAsync((User?)null);

            var result = await _controller.Withdraw(withdrawDto);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User not found.", notFoundResult.Value);
        }

        [Fact]
        public async Task Withdraw_InvalidPassword_ReturnsUnauthorized()
        {
            var withdrawDto = new WithdrawDto
            {
                AccountNumber = "12345",
                Amount = 50,
                Password = "wrongpassword"
            };

            var user = new User { Id = "user1", Email = "test@example.com" };

            _mockTransactionRepository.Setup(repo => repo.GetUserByAccountNumberAsync(withdrawDto.AccountNumber))
                .ReturnsAsync(user);
            _mockUserManager.Setup(um => um.CheckPasswordAsync(user, withdrawDto.Password))
                .ReturnsAsync(false);

            var result = await _controller.Withdraw(withdrawDto);

            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid password.", unauthorizedResult.Value);
        }
    }
}
