using System.ComponentModel.DataAnnotations;

namespace LibraryApi.DTOs
{
    public class RegisterRequest
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(4)]
        public string Password { get; set; } = string.Empty;
    }
}