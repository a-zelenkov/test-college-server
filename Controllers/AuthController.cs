using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using API.Models;
using System.Threading.Tasks;
using System;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DataBaseContext db;
        public AuthController(DataBaseContext context)
        {
            db = context;
        }

        [HttpGet]
        [Route("me")]
        public async Task<ActionResult> ProfileInfo()
        {
            if (HttpContext.Request.Headers.ContainsKey("Authorization"))
            {
                string token = HttpContext.Request.Headers["Authorization"];

                User user = await db.Users.FirstOrDefaultAsync(x => x.Token == token);

                if (user != null)
                {
                    return Ok(user);
                }
            }
            return Unauthorized("\"Unauthorized\"");
        }

        // add validation
        [HttpPost]
        [Route("me")]
        public async Task<ActionResult<User>> ChangeProfile(User data)
        {

            User user = await db.Users.FirstOrDefaultAsync(x => x.Id == data.Id);

            if (data.Password != "")
            {
                user.Password = GetHash(data.Password);
            }

            user.Login = data.Login;
            user.Name = data.Name;
            user.School = data.School;
            user.Phone = data.Phone;

            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            return Ok(user);
        }


        [HttpGet]
        [Route("logout")]
        public async Task<ActionResult> LogOut()
        {
            if (HttpContext.Request.Headers.ContainsKey("Authorization"))
            {
                string token = HttpContext.Request.Headers["Authorization"];

                User user = await db.Users.FirstOrDefaultAsync(x => x.Token == token);

                if (user != null)
                {
                    string newToken = GetHash($"{user.Login}{DateTime.Now}{user.Phone}");

                    System.IO.File.WriteAllText("C:\\Users\\PC\\Desktop\\VISUAL STUDIO\\log.txt", DateTime.Now.ToString());

                    TokenResponse response = new()
                    {
                        Token = newToken
                    };

                    user.Token = token;

                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();

                    return Ok(response);
                }
            }

            return Unauthorized("\"Unauthorized\"");
        }

        // POST api/auth/registration
        [HttpPost]
        [Route("registration")]
        public async Task<ActionResult<User>> Register(User user)
        {
            if (await db.Users.FirstOrDefaultAsync(x => x.Login == user.Login) != null)
            {
                ModelState.AddModelError("login", "Этот логин уже занят");
            }

            if (await db.Users.FirstOrDefaultAsync(x => x.Phone == user.Phone) != null)
            {
                ModelState.AddModelError("phone", "Этот номер телефона уже был использован");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            user.Password = GetHash(user.Password);
            user.Token = GetHash($"{user.Login}{DateTime.Now}{user.Phone}");

            db.Users.Add(user);
            db.SaveChanges();

            TokenResponse response = new()
            {
                Token = user.Token
            };

            return Ok(response);
        }

        // POST api/auth/login
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Login(UserLogin data)
        {
            User user = await db.Users.FirstOrDefaultAsync(x => x.Login == data.Login && x.Password == GetHash(data.Password));         

            if (user == null)
            {
                ModelState.AddModelError("login", "Не удалось войти");
                ModelState.AddModelError("password", "Неверно указан логин или пароль");
                return BadRequest(ModelState);
            }

            string token = GetHash($"{user.Login}{DateTime.Now}{user.Phone}");


            TokenResponse response = new()
            {
                Token = token
            };

            user.Token = token;

            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            return Ok(response);
        }

        private static string GetHash(string input)
        {
            input += "sqfweg342tfw24t";
            SHA512 sha = SHA512.Create();
            byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(input));

            return Convert.ToBase64String(hash);
        }
    }

}

/*            System.IO.File.AppendAllText("C:\\Users\\PC\\Desktop\\VISUAL STUDIO\\log.txt", token);*/