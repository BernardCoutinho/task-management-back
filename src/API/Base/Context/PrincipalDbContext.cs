using task_management.src.API.Model;
using Microsoft.EntityFrameworkCore;
using TaskItem = task_management.src.API.Model.TaskItem;

namespace task_management.src.API.Base.Context
{
    public class PrincipalDbContext : DbContext
    {
        public PrincipalDbContext(DbContextOptions<PrincipalDbContext> options) : base(options)
        {
        }

        public DbSet<User> User { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.HasKey(e => e.Id);
               
                entity.Property(e => e.Id)
                      .ValueGeneratedOnAdd();
                
                entity.Property(e => e.Username)
                      .IsRequired()
                      .HasMaxLength(100);
               
                entity.Property(e => e.Email)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(e => e.PasswordHash)
                      .IsRequired();

                entity.HasMany(u => u.Tasks)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TaskItem>(entity =>
            {
                entity.ToTable("Tasks");

                entity.HasKey(t => t.Id);

                entity.Property(t => t.Title)
                        .IsRequired();        
                      
                entity.Property(t => t.Description)
                        .IsRequired();

                entity.Property(t => t.Completed)
                        .IsRequired();       
            });
        }
    }
}
