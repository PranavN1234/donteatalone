using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDTO{

        [Required]
        public string Username{get; set;}

        [Required]
        public string Knownas { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public DateOnly? DateofBirth { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        [StringLength(8, MinimumLength =4)]
        public string Password { get; set; }
    }
}