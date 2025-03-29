using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the User as the base entity for TPH (Table-per-Hierarchy) inheritance
            modelBuilder.Entity<User>()
                .ToTable("Users")
                .HasDiscriminator<string>("UserType")
                .HasValue<Student>("Student")
                .HasValue<Teacher>("Teacher")
                .HasValue<Admin>("Admin");

            // Configure properties
            modelBuilder.Entity<User>()
                .Property(u => u.Username)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.FirstName)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.LastName)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.Password)
                .IsRequired();

            // Add a unique constraint on Username
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // Configure StudentCourse (Many-to-Many relationship)
            modelBuilder.Entity<StudentCourse>()
                .HasKey(sc => new { sc.StudentId, sc.CourseId });

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Student)
                .WithMany()
                .HasForeignKey(sc => sc.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Course)
                .WithMany(c => c.StudentCourses)
                .HasForeignKey(sc => sc.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Grade relationships
            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Student)
                .WithMany()
                .HasForeignKey(g => g.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Course)
                .WithMany()
                .HasForeignKey(g => g.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}