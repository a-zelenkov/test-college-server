using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Login { get; set; }

        public string Password { get; set; }

        public string Social { get; set; }

        [Required]
        public string School { get; set; }

        [Required]
        public string Phone { get; set; }

        public string Token { get; set; }


    }


    public class UserLogin
    {
        [Required]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }

    }

    public class TokenResponse
    {
        public string Token { get; set; }

    }

}
