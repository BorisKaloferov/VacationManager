using Business_Layer;
using Microsoft.EntityFrameworkCore;

namespace Data_Layer
{
    public class VacationManagerDbContext : DbContext 
    {
        public VacationManagerDbContext() : base()
        {
            
        }

        public VacationManagerDbContext(DbContextOptions options) : base(options)
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=LAPTOP-UNHGGLSQ\\SQLEXPRESS;Database=VacationManagerDb;Trusted_Connection=True;TrustServerCertificate=True;");

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Team>()
                .HasOne(t => t.Leader)
                .WithMany()
                .HasForeignKey(t => t.LeaderId)
                .OnDelete(DeleteBehavior.Restrict); 

      
            modelBuilder.Entity<User>()
                .HasOne(u => u.Team)
                .WithMany(t => t.Users)
                .HasForeignKey(u => u.TeamId)
                .OnDelete(DeleteBehavior.SetNull);
        }

        public DbSet<Project> Projects { get; set; }

        public DbSet<Vacation> Vacations { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Team> Teams { get; set; }
    }
}
