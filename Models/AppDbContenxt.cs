using Learnify.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using practice.Models;

namespace learnify.Models
{
    public class AppDbContenxt : IdentityDbContext<Users>
    {
        public AppDbContenxt(DbContextOptions<AppDbContenxt> options) : base(options) { }

        public DbSet<Classes> Classes { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enroll { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Quiz> Quizs { get; set; }
        public DbSet<Result> Result { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
    }
}