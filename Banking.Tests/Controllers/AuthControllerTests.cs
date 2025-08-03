using Banking.Controllers;
using Banking.DTO;
using Banking.Entity;
using Banking.Repositories;
using Banking.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Banking.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<SignInManager<User>> _mockSignInManager;
        private readonly Mock<IJwtService> _mockJwtService;
        private readonly Mock<IAccountRepository> _mockAccountRepository;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            var mockUserStore = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(
                mockUserStore.Object, null, null, null, null, null, null, null, null);

            var mockContextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
            var mockUserPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<User>>();
            _mockSignInManager = new Mock<SignInManager<User>>(
                _mockUserManager.Object, mockContextAccessor.Object, mockUserPrincipalFactory.Object, null, null, null, null);

            _mockJwtService = new Mock<IJwtService>();
            _mockAccountRepository = new Mock<IAccountRepository>();

            _controller = new AuthController(
                _mockUserManager.Object,
                _mockSignInManager.Object,
                _mockJwtService.Object,
                _mockAccountRepository.Object);
        }

        [Fact]
        public async Task Login_InvalidUser_ReturnsUnauthorized()
        {
            var loginDto = new LoginDto { Email = "nonexistent@example.com", Password = "password123" };

            _mockUserManager.Setup(um => um.FindByEmailAsync(loginDto.Email))
                .ReturnsAsync((User?)null);

            var result = await _controller.Login(loginDto);

            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var response = Assert.IsType<LoginResponseDto>(unauthorizedResult.Value);
            Assert.False(response.IsSuccessful);
            Assert.Equal("Invalid email or password.", response.ErrorMessage);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOkWithToken()
        {
            var loginDto = new LoginDto { Email = "test@example.com", Password = "password123" };
            var user = new User { Id = "user1", Email = loginDto.Email, UserName = "testuser" };
            var roles = new List<string> { "User" };
            var token = "fake-jwt-token";

            _mockUserManager.Setup(um => um.FindByEmailAsync(loginDto.Email))
                .ReturnsAsync(user);
            _mockSignInManager.Setup(sm => sm.CheckPasswordSignInAsync(user, loginDto.Password, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
            _mockUserManager.Setup(um => um.GetRolesAsync(user))
                .ReturnsAsync(roles);
            _mockJwtService.Setup(js => js.GenerateToken(user, roles))
                .Returns(token);

            var result = await _controller.Login(loginDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<LoginResponseDto>(okResult.Value);
            Assert.True(response.IsSuccessful);
            Assert.Equal(token, response.Token);
        }

        [Fact]
        public async Task Login_InvalidPassword_ReturnsUnauthorized()
        {
            var loginDto = new LoginDto { Email = "test@example.com", Password = "wrongpassword" };
            var user = new User { Id = "user1", Email = loginDto.Email };

            _mockUserManager.Setup(um => um.FindByEmailAsync(loginDto.Email))
                .ReturnsAsync(user);
            _mockSignInManager.Setup(sm => sm.CheckPasswordSignInAsync(user, loginDto.Password, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            var result = await _controller.Login(loginDto);

            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var response = Assert.IsType<LoginResponseDto>(unauthorizedResult.Value);
            Assert.False(response.IsSuccessful);
        }

    }
}
