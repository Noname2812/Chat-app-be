
using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models.RequestModels
{
    public class LoginModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}
