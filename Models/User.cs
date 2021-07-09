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

        [Required]
        public string School { get; set; }

        [Required]
        public string Phone { get; set; }

        public string Token { get; set; }

        public bool IsTested { get; set; }

    }

    public class TestResult
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int Result { get; set; }

        public int AnswersCount { get; set; }

        public int QuestionsCount { get; set; }
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
