using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Entities;
namespace UserManagement.Infrastructure.DBContext
{
    public class UserManagementContext : DbContext
    {
        public UserManagementContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<User> Users {  get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>().HasQueryFilter(s => !s.IsDeleted);

            base.OnModelCreating(builder);
        }
        }
}
