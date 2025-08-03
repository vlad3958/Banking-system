namespace Banking.DTO
{
    public class RegistrationResponseDto
    {
        public bool IsSuccessfulRegistration{ get; set; }

        public IEnumerable<string>? Errors { get; set; }
        public string? AccountNumber { get; set; }
    }
}
