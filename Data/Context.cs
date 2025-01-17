using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class Context : BaseContext
    {
        public DbSet<User> Users { get; set; }

        public Context() : base()
        {
        }

        public Context(DbContextOptions<BaseContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasMany(u => u.SentMessages).WithOne(m => m.FromUser).HasForeignKey(m => m.FromId);
            modelBuilder.Entity<User>().HasMany(u => u.ReceivedMessages).WithOne(m => m.ToUser).HasForeignKey(m => m.ToId);

            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                CreationTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                IsDeleted = false,
                Username = "admin",
                //RockAndStoneMyBrother
                Password = @"$2a$10$xONPZP9LFxq49VXuO3uuZ.IPcWHkAXsrsGtM.uWRgQX89DSx0JPrK",
                PublicKey = "test"
            });
        }
    }
}
