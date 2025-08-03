using Banking.DTO;
using Banking.Entity;
using System.Threading.Tasks;

namespace Banking.Repositories
{
    public interface IAccountRepository
    {
        Task CreateAccountAsync(Account account);
        Task<IEnumerable<AccountResponseSimpleDto>> GetAllAccountsAsync();
        Task<AccountResponseFullDto?> GetAccountByAccountNumberAsync(string accountNumber);
        Task<Account> GetAccountByUserIdAsync(string id);
    }
}
