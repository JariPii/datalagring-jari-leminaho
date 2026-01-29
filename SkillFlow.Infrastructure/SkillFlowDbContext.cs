using Microsoft.EntityFrameworkCore;
using SkillFlow.Domain.Entities.Attendees;
using SkillFlow.Domain.Entities.Courses;
using SkillFlow.Domain.Entities.CourseSessions;
using SkillFlow.Domain.Entities.Locations;

namespace SkillFlow.Infrastructure
{
    public class SkillFlowDbContext : DbContext
    {
        public SkillFlowDbContext(DbContextOptions<SkillFlowDbContext> options) : base(options) { }
        
        public DbSet<Attendee> Attendees { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Competence> Competences { get; set; }
        public DbSet<CourseSession> CourseSessions { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Location> Locations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SkillFlowDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
