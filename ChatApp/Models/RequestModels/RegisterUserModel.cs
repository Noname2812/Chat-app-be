using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models.RequestModels
{
    public class RegisterUserModel
    {
        [Required]
        public string Name { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [MinLength(10)]
        [MaxLength(11)]
        public string Phone { get; set; }
        public string Address { get; set; }
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
        [Required]
        [MaxLength(30)]
        public string UserName { get; set; }
    }
}
