using Banking.DTO;
using Banking.Entity;

namespace Banking.Repositories
{
    public interface ITransactionRepository
    {
        Task<bool> DepositAsync(DepositDto depositDto);
        Task<User> GetUserByAccountNumberAsync(string? accountNumber);
        Task<bool> TransferAsync(TransferDto transferDto);
        Task<bool> WithdrawAsync(WithdrawDto withdrawDto);
    }
}
