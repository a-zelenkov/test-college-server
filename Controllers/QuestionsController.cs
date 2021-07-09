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

                User user = await db.Users.FirstOrDefaultAsync(x => x.Token == token);

                if (user != null)
                {
                    if (user.IsTested)
                    {
                        return Ok(await db.TestResults.FirstOrDefaultAsync(x => x.UserId == user.Id));
                    };

                    List<QuestionSecret> response = new();

                    List<Question> questions = await db.Questions.ToListAsync();

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

        [HttpPost]
        public async Task<ActionResult> PostQuestion(QuestionBuilder data)
        {
            Question question = data.Question;

            await db.Questions.AddAsync(question);

            await db.SaveChangesAsync();

            foreach (Answer answer in data.Answers)
            {
                answer.QuestionId = question.Id;
                await db.Answers.AddAsync(answer);
            }

            await db.SaveChangesAsync();

            return Ok("\"Success\"");
        }

        // POST api/questions/results
        [HttpPost]
        [Route("results")]
        public async Task<ActionResult> PostResults(List<QuestionResult> data)
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
                        Result = 0,
                        QuestionsCount = db.Questions.Count(),
                        AnswersCount = data.Count
                    };

                    foreach (QuestionResult answer in data)
                    {
                        if (await db.Answers.FirstOrDefaultAsync(x => x.Id == answer.Id && x.IsCorrect) != null)
                        {
                            result.Result++;
                        }
                    }

                    user.IsTested = true;

                    db.Entry(user).State = EntityState.Modified;
                    db.TestResults.Add(result);
                    db.SaveChanges();

                    return Ok(result);
                }
            }

            return Unauthorized("\"Unauthorized\"");
            
        }

    }   

}

/*            System.IO.File.AppendAllText("C:\\Users\\PC\\Desktop\\VISUAL STUDIO\\log.txt", token);*/