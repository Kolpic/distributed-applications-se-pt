using Common.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Repositories
{
    public class ProjectManagementDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProjectCategory> ProjectCategories { get; set; }
        public DbSet<Comment> Comments { get; set; }

        public ProjectManagementDbContext()
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure many-to-many relationship
            modelBuilder.Entity<ProjectCategory>()
                .HasKey(pc => new { pc.ProjectId, pc.CategoryId }); // Unique key's ProjectId, CategoryId

            modelBuilder.Entity<ProjectCategory>()
                .HasOne(pc => pc.Project)
                .WithMany(p => p.ProjectCategories)
                .HasForeignKey(pc => pc.ProjectId);

            modelBuilder.Entity<ProjectCategory>()
                .HasOne(pc => pc.Category)
                .WithMany(c => c.ProjectCategories)
                .HasForeignKey(pc => pc.CategoryId);

            // Configure one-to-many relationship for Comments
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Project)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.ProjectId);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseMySql(
                "Server=localhost;" +
                "Port=3306;" +
                "Database=web2;" +
                "User=root;" +
                "Password=1234;",
                new MySqlServerVersion(new Version(8, 0, 11))
                );
        }
    }
}
