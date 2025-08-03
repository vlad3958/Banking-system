using Banking.DTO;
using Banking.Entity;
using Microsoft.EntityFrameworkCore;

namespace Banking.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly DbContext.DatabaseContext _dbContext;

        public TransactionRepository(DbContext.DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task<User> GetUserByAccountNumberAsync(string? accountNumber)
        {
            if (string.IsNullOrEmpty(accountNumber))
            {
                throw new ArgumentException("Account number cannot be null or empty.", nameof(accountNumber));
            }
            var account = await _dbContext.Accounts
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
            return account?.User;
        }

        public async Task<bool> TransferAsync(TransferDto transferDto)
        {
            var fromAccount = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == transferDto.FromAccountNumber);
            var toAccount = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == transferDto.ToAccountNumber);

            if (fromAccount == null || toAccount == null)
            {
                return false; // Accounts not found
            }
            if (fromAccount.Balance < transferDto.Amount)
            {
                return false; // Insufficient funds
            }
            fromAccount.Balance -= transferDto.Amount;
            toAccount.Balance += transferDto.Amount;

            _dbContext.Accounts.Update(fromAccount);
            _dbContext.Accounts.Update(toAccount);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DepositAsync(DepositDto depositDto)
        {
            var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == depositDto.AccountNumber);
            if (account == null)
            {
                return false;
            }
            account.Balance += depositDto.Amount;
            _dbContext.Accounts.Update(account);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> WithdrawAsync(WithdrawDto withdrawDto)
        {
            var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == withdrawDto.AccountNumber);
          
            if (account == null)
            {
                return false;
            }
            if (account.Balance < withdrawDto.Amount)
            {
                return false; // Insufficient funds
            }
            account.Balance -= withdrawDto.Amount;
            _dbContext.Accounts.Update(account);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
   }
