using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Answer
    {
        public int Id { get; set; }

        [Required]
        public string Value { get; set; }

        public int QuestionId { get; set; }

        [Required]
        public bool IsCorrect { get; set; }

        public PreparedAnswer ToPreparedAnswer()
        {
            return new PreparedAnswer()
            {
                Id = Id,
                Value = Value,
                IsSelected = false
            };
        }

    }

    public class PreparedAnswer
    {
        public int Id { get; set; }

        public string Value { get; set; }

        public bool IsSelected { get; set; }
    }

}
