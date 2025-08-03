using AutoMapper;
using Banking.Entity;
using Banking.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Banking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IAccountRepository _accountRepository;

        public AccountsController(UserManager<User> userManager, IAccountRepository accountRepository)
        {
            _userManager = userManager;
            _accountRepository = accountRepository;
        }

        [HttpGet("GetAllAccounts")]
        public async Task<IActionResult> GetAllAccounts()
        {
            var limitedAccounts = await _accountRepository.GetAllAccountsAsync();
            return Ok(limitedAccounts);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetAccountByAccountNumber/{accountNumber}")]
        public async Task<IActionResult> GetAccountByAccountNumber(string accountNumber)
        {
            var account = await _accountRepository.GetAccountByAccountNumberAsync(accountNumber);
            if (account == null)
            {
                return NotFound("Account not found.");
            }
            return Ok(account);
        }

[HttpGet("myAccountInfo")]
        public async Task<IActionResult> GetMyAccountInfo(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var result = await _userManager.CheckPasswordAsync(user, password);
            if (!result)
            {
                return Unauthorized("Invalid password.");
            }

            var account = await _accountRepository.GetAccountByUserIdAsync(user.Id);
            if (account == null)
            {
                return NotFound("Account not found.");
            }

            return Ok(account);
        }
    }
}
