using Banking.DTO;
using Banking.Entity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Banking.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly DbContext.DatabaseContext _dbContext;

        public AccountRepository(DbContext.DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateAccountAsync(Account account)
        {
            _dbContext.Accounts.Add(account);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<AccountResponseSimpleDto>> GetAllAccountsAsync()
        {
            return await _dbContext.Accounts.Select(a => new AccountResponseSimpleDto
            {
                Id = a.Id,
                FirstName = a.User != null ? a.User.FirstName : "Unknown",
                LastName = a.User != null ? a.User.LastName : "Unknown",
                AccountNumber = a.AccountNumber ?? string.Empty,
            }).ToListAsync();
        }

        public async Task<AccountResponseFullDto?> GetAccountByAccountNumberAsync(string accountNumber)
        {
            return await _dbContext.Accounts.Where(a => a.AccountNumber == accountNumber).Select(a => new AccountResponseFullDto
            {
                Id = a.Id,
                AccountNumber = a.AccountNumber ?? string.Empty,
                Balance = a.Balance,
                User = a.User != null ? a.User : new User
                {
                    FirstName = "Unknown",
                    LastName = "Unknown",
                    Email = "Unknown"
                },
                UserId = a.UserId
            }).FirstOrDefaultAsync();
        }

        public async Task<Account> GetAccountByUserIdAsync(string id)
        {
            var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.UserId == id);
            if (account == null)
            {
                throw new KeyNotFoundException("Account not found for the given user ID.");
            }
            return account;
        }
    }
}
