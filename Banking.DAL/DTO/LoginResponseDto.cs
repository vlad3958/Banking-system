namespace Banking.DTO
{
    public class LoginResponseDto
    {
        public bool IsSuccessful { get; set; }
        public string? Token { get; set; }
        public string? ErrorMessage { get; set; }
        public bool IsAdmin { get; set; }
    }
}
