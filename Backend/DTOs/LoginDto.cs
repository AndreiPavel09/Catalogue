using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs
{
        public class LoginDto
        {
            [Required]
            public string Username { get; set; }

            [Required]
            public string Password { get; set; }
        }
}
