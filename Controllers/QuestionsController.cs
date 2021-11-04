using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using API.Models;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionsController : ControllerBase
    {
        private readonly DataBaseContext db;

        public QuestionsController(DataBaseContext context)
        {
            db = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllQuestions()
        {
            if (HttpContext.Request.Headers.ContainsKey("Authorization"))
            {
                string token = HttpContext.Request.Headers["Authorization"];
                int testId = 0;

                if (HttpContext.Request.Query.ContainsKey("test_id"))
                {
                    testId = int.Parse(HttpContext.Request.Query["test_id"]);
                }
            
                User user = await db.Users.FirstOrDefaultAsync(x => x.Token == token);

                if (user != null)
                {

                    TestResult testResult = await db.TestResults.FirstOrDefaultAsync(x => x.UserId == user.Id && x.TestId == testId);

                    bool isTested = testResult != null;

                    if (isTested)
                    {
                        PrepearedTestResult prepearedTestResult = testResult.ToPrepearedTestResult();
                        prepearedTestResult.Info = db.TestInfos.FirstOrDefault(x => x.Id == testResult.InfoId);
                        return Ok(prepearedTestResult);
                    };

                    List<QuestionSecret> response = new();

                    List<Question> questions = await db.Questions.Where(x => x.TestId == testId).ToListAsync();

                    foreach (Question question in questions)
                    {
                        QuestionSecret questionBuilder = new() 
                        {
                            Id = question.Id,
                            Question = question,
                            Answers = new()
                        };

                        List<Answer> answers = await db.Answers.Where(x => x.QuestionId == question.Id)
                                                    .ToListAsync();

                        foreach (Answer answer in answers)
                        {
                            questionBuilder.Answers.Add(answer.ToPreparedAnswer());
                        }

                        response.Add(questionBuilder);

                    }

                    return Ok(response);
                }
            }

            return Unauthorized("\"Unauthorized\"");
        }

        [HttpGet]
        [Route("{id}/answers")]
        public async Task<Array> GetAnswers(int id)
        {
            return await db.Answers.Where(x => x.QuestionId == id).ToArrayAsync();
        }

        [HttpGet]
        [Route("tests")]
        public async Task<Array> GetTests()
        {
            return await db.Tests.ToArrayAsync();
        }

        [HttpPost]
        public async Task<ActionResult> PostQuestion(TestBuilder data)
        {

            Test test = new()
            {
                Title = data.Title,
                Description = data.Description,
                Type = data.Type
            };

            await db.Tests.AddAsync(test);
            await db.SaveChangesAsync();

            foreach (QuestionBuilder questionBuilder in data.Questions)
            {
                Question question = new()
                {
                    TestId = test.Id,
                    Value = questionBuilder.Value,
                };

                await db.Questions.AddAsync(question);
                await db.SaveChangesAsync();

                foreach (Answer answer in questionBuilder.Answers)
                {
                    answer.QuestionId = question.Id;
                    await db.Answers.AddAsync(answer);
                    await db.SaveChangesAsync();
                }
            }

            return Ok("\"Success\"");
        }

        // POST api/questions/results
        [HttpPost]
        [Route("results")]
        public async Task<ActionResult> PostResults(QuestionResult data)
        {
            if (HttpContext.Request.Headers.ContainsKey("Authorization"))
            {
                string token = HttpContext.Request.Headers["Authorization"];

                User user = await db.Users.FirstOrDefaultAsync(x => x.Token == token);

                if (user != null)
                {
                    TestResult result = new()
                    {
                        UserId = user.Id,
                        TestId = data.TestId,
                        Type = db.Tests.FirstOrDefault(x => x.Id == data.TestId).Type,
                        Score = 0,
                        MostPopular = 0,
                        QuestionsCount = data.QuestionsCount,
                        AnswersCount = data.Answers.Count
                    };

                    List<Answer> answersList = new();

                    foreach (AnswerId answer in data.Answers)
                    {
                        Answer answerInfo = await db.Answers.FirstOrDefaultAsync(x => x.Id == answer.Id);
                        result.Score += answerInfo.Score;

                        answersList.Add(answerInfo);
                    }

                    var answers = answersList.Where(x => x.Score > 0);

                    if (answers.Count() > 0)
                    {
                        result.MostPopular = answers.GroupBy(x => x.Score).FirstOrDefault().Key;
                    }

                    if (result.Type == "graph")
                    {
                        TestInfo testInfo = new()
                        {
                            EducationType = answers.ToList().Find(x => x.Score == 1000).Value,
                            EducationPlace = answers.ToList().Find(x => x.Score == 1001).Value,
                            Graph1 = answers.Where(x => x.Score == 1).Count() * 11,
                            Graph2 = answers.Where(x => x.Score == 2).Count() * 11 + 1,
                            Graph3 = answers.Where(x => x.Score == 3).Count() * 11,
                            Graph4 = answers.Where(x => x.Score == 4).Count() * 11,
                            Graph5 = answers.Where(x => x.Score == 5).Count() * 11,
                            Graph6 = answers.Where(x => x.Score == 6).Count() * 11,
                        };

                        db.TestInfos.Add(testInfo);
                        db.SaveChanges();

                        result.InfoId = testInfo.Id;
                    }
                  
                    db.Entry(user).State = EntityState.Modified;
                    db.TestResults.Add(result);
                    db.SaveChanges();

                    PrepearedTestResult testResult = result.ToPrepearedTestResult();
                    testResult.Info = db.TestInfos.FirstOrDefault(x => x.Id == result.InfoId);

                    return Ok(testResult);
                }
            }

            return Unauthorized("\"Unauthorized\"");
            
        }

    }   

}

/*            System.IO.File.AppendAllText("C:\\Users\\PC\\Desktop\\VISUAL STUDIO\\log.txt", token);*/