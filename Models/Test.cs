using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Test
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Type { get; set; }

        public string Description { get; set; }
    }

    public class TestBuilder
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Type { get; set; }

        public string Description { get; set; }

        public List<QuestionBuilder> Questions { get; set; }
    }

    public class TestResult
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int TestId { get; set; }

        public int Score { get; set; }

        public string Type { get; set; }

        public int MostPopular { get; set; }

        public int AnswersCount { get; set; }

        public int QuestionsCount { get; set; }

        public int? InfoId { get; set; }

        public PrepearedTestResult ToPrepearedTestResult()
        {
            return new PrepearedTestResult()
            {
                Score = Score,
                Type = Type,
                MostPopular = MostPopular,
                QuestionsCount = QuestionsCount,
                AnswersCount = AnswersCount,              
            };
        }
    }

    public class PrepearedTestResult
    {
        public int Score { get; set; }

        public string Type { get; set; }

        public int MostPopular { get; set; }

        public int AnswersCount { get; set; }

        public int QuestionsCount { get; set; }

        public TestInfo Info { get; set; }

    }

    public class TestInfo
    {
        public int Id { get; set; }

        public string EducationType { get; set; }

        public string EducationPlace { get; set; }

        public int Graph1 { get; set; }

        public int Graph2 { get; set; }

        public int Graph3 { get; set; }

        public int Graph4 { get; set; }

        public int Graph5 { get; set; }

        public int Graph6 { get; set; }
    }

}