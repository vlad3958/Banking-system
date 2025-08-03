namespace Banking.DTO
{
    public class AccountResponseDto
    {
        public int Id { get; set; }
        public string? AccountNumber { get; set; }
        // Excluded sensitive information like Balance, User details
    }
}