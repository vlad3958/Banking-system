namespace Banking.DTO
{
    public class WithdrawDto
    {
        public string? AccountNumber { get; set; }
        public string? Password { get; set; }
        public decimal Amount { get; set; }
    }
}
