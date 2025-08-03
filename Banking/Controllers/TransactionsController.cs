using Banking.DTO;
using Banking.Entity;
using Banking.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Banking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionRepository _transactionRepository;

        private readonly UserManager<User> _userManager;
        public TransactionsController(ITransactionRepository transactionRepository, UserManager<User> userManager)
        {
            _transactionRepository = transactionRepository;
            _userManager = userManager;
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferDto transferDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid transfer data.");
            }

            var result = await _transactionRepository.TransferAsync(transferDto);
            if (!result)
            {
                return BadRequest("Transfer failed. Please check account numbers and balance.");
            }

            return Ok(new { Message = "Transfer successful." });
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] DepositDto depositDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid deposit data.");
            }
            var result = await _transactionRepository.DepositAsync(depositDto);
            if (!result)
            {
                return BadRequest("Deposit failed. Please check account number.");
            }
            return Ok(new { Message = "Deposit successful." });
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] WithdrawDto withdrawDto)
        {
            var user = await _transactionRepository.GetUserByAccountNumberAsync(withdrawDto.AccountNumber);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var check = await _userManager.CheckPasswordAsync(user, withdrawDto.Password);
            if (!check)
            {
                return Unauthorized("Invalid password.");
            }

            var result = await _transactionRepository.WithdrawAsync(withdrawDto);
            if (!result)
            {
                return BadRequest("Withdraw failed. Please check account number and balance.");
            }

            return Ok(new { Message = "Withdraw successful." });
        }
    }
}
