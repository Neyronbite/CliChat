using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace Data
{
    public class BaseContext : DbContext
    {
        protected readonly string connectionString;
        public BaseContext()
        {
            OnCreation();
        }

        public BaseContext(DbContextOptions<BaseContext> options) : base(options)
        {
            OnCreation();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (!optionsBuilder.IsConfigured)
            {
                //TODO use configuration
                var path = Environment.GetEnvironmentVariable("DB_VOL");
                path = path == null ? Environment.GetEnvironmentVariable("DB_VOL", EnvironmentVariableTarget.Machine) : path;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                optionsBuilder.UseSqlite($"Data Source={path}\\ChatDb.db;");

                optionsBuilder.UseSnakeCaseNamingConvention();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasAnnotation("Relational:Collation", "en_US.utf8");
        }

        private void OnCreation()
        {
            Batteries.Init();
            Database.EnsureCreated();
            //Database.Migrate();
        }
    }
}
