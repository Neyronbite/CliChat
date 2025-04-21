using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class Context : BaseContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }

        public Context() : base()
        {
        }

        public Context(DbContextOptions<BaseContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // I am commenting this line, because I realize that 
            // it will not work with temporary groups
            //modelBuilder.Entity<User>().HasMany(u => u.SentMessages).WithOne(m => m.FromUser).HasForeignKey(m => m.FromId);
            modelBuilder.Entity<User>().HasMany(u => u.ReceivedMessages).WithOne(m => m.ToUser).HasForeignKey(m => m.ToId);
        }
    }
}
