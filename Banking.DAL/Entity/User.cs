using Microsoft.AspNetCore.Identity;

namespace Banking.Entity
{
    public class User : IdentityUser
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }
    }
}
