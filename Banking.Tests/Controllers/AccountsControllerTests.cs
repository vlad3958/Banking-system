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
    public class AccountsControllerTests
    {
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<IAccountRepository> _mockAccountRepository;
        private readonly AccountsController _controller;

        public AccountsControllerTests() 
        {
            var mockUserStore = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(
                mockUserStore.Object, null, null, null, null, null, null, null, null);
            
            _mockAccountRepository = new Mock<IAccountRepository>();
            _controller = new AccountsController(_mockUserManager.Object, _mockAccountRepository.Object);
        }

        [Fact]
        public async Task GetAllAccounts_ReturnsOkResult_WithListOfAccounts()
        {
            var mockAccounts = new List<AccountResponseSimpleDto>
            {
                new AccountResponseSimpleDto { Id = 1, FirstName = "John", LastName = "Doe", AccountNumber = "12345" },
                new AccountResponseSimpleDto { Id = 2, FirstName = "Jane", LastName = "Smith", AccountNumber = "67890" }
            };
            _mockAccountRepository.Setup(repo => repo.GetAllAccountsAsync())
                .ReturnsAsync(mockAccounts);

            var result = await _controller.GetAllAccounts();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedAccounts = Assert.IsAssignableFrom<IEnumerable<AccountResponseSimpleDto>>(okResult.Value);
            Assert.Equal(2, returnedAccounts.Count());
        }

        [Fact]
        public async Task GetAccountByAccountNumber_ExistingAccount_ReturnsOkResult()
        {
            var accountNumber = "12345";
            var mockAccount = new AccountResponseFullDto
            {
                Id = 1,
                AccountNumber = accountNumber,
                Balance = 1000,
                UserId = "user1"
            };
            _mockAccountRepository.Setup(repo => repo.GetAccountByAccountNumberAsync(accountNumber))
                .ReturnsAsync(mockAccount);

            var result = await _controller.GetAccountByAccountNumber(accountNumber);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedAccount = Assert.IsType<AccountResponseFullDto>(okResult.Value);
            Assert.Equal(accountNumber, returnedAccount.AccountNumber);
        }

        [Fact]
        public async Task GetAccountByAccountNumber_NonExistingAccount_ReturnsNotFound()
        {
            var accountNumber = "99999";
            _mockAccountRepository.Setup(repo => repo.GetAccountByAccountNumberAsync(accountNumber))
                .ReturnsAsync((AccountResponseFullDto?)null);

            var result = await _controller.GetAccountByAccountNumber(accountNumber);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Account not found.", notFoundResult.Value);
        }

        [Fact]
        public async Task GetMyAccountInfo_InvalidUser_ReturnsNotFound()
        {
            var email = "nonexistent@example.com";
            var password = "password123";

            _mockUserManager.Setup(um => um.FindByEmailAsync(email))
                .ReturnsAsync((User?)null);

            var result = await _controller.GetMyAccountInfo(email, password);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User not found.", notFoundResult.Value);
        }

        [Fact]
        public async Task GetMyAccountInfo_InvalidPassword_ReturnsUnauthorized()
        {
            var email = "test@example.com";
            var password = "wrongpassword";
            var user = new User { Id = "user1", Email = email };

            _mockUserManager.Setup(um => um.FindByEmailAsync(email))
                .ReturnsAsync(user);
            _mockUserManager.Setup(um => um.CheckPasswordAsync(user, password))
                .ReturnsAsync(false);

            var result = await _controller.GetMyAccountInfo(email, password);

            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid password.", unauthorizedResult.Value);
        }
    }
}
