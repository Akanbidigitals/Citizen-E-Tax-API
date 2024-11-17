using Citizen_E_Tax_API.Models.Domain;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using Document = Citizen_E_Tax_API.Models.Domain.Document;

namespace Citizen_E_Tax_API.DataAccess.DataContext
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Role>().HasData(new Role { Id = 1, Name = "Admin" }, new Role { Id = 2, Name = "User" });
            modelBuilder.Entity<UserRole>().HasKey(k => new { k.RoleId, k.UserId });
            modelBuilder.Entity<UserRole>().HasOne(k => k.User).WithMany(c => c.Roles).HasForeignKey(k => k.UserId);
            modelBuilder.Entity<UserRole>().HasOne(k => k.Role).WithMany( c => c.Users).HasForeignKey(c => c.RoleId);

        }



    }
}
