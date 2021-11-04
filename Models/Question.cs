using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Question
    {
        public int Id { get; set; }

        public int TestId { get; set; }

        [Required]
        public string Value { get; set; }

    }

    public class QuestionBuilder
    {
        public string Value { get; set; }

        [Required]
        public List<Answer> Answers { get; set; }

    }

    public class QuestionSecret
    {
        public int Id { get; set; }

        [Required]
        public Question Question { get; set; }

        [Required]
        public List<PreparedAnswer> Answers { get; set; }

    }

    public class QuestionResult
    {
        public int TestId { get; set; }

        public int QuestionsCount { get; set; }

        public List<AnswerId> Answers { get; set; }
    }

}
