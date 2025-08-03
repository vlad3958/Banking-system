using Banking.Entity;

namespace Banking.DTO
{
    public class AccountResponseFullDto
    {
        public int Id { get; set; }
        public string? AccountNumber { get; set; }
        public decimal? Balance { get; set; }
        public User? User { get; set; }
        public string? UserId { get; set; }
    }

}
