using System.ComponentModel.DataAnnotations;

namespace Banking.DTO
{
    public class AssignRoleDto
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; } = string.Empty;
    }
}
