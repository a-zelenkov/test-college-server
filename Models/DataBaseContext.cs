using Microsoft.EntityFrameworkCore;

namespace API.Models
{
    public class DataBaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<TestResult> TestResults { get; set; }

        public DataBaseContext(DbContextOptions<DataBaseContext> options)
            : base(options)
        {
/*            Database.EnsureDeleted();
            Database.EnsureCreated();*/
        }
    }
}
