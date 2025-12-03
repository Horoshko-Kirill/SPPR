using Microsoft.EntityFrameworkCore;

namespace WEB_353505_Horoshko.Context
{
    public class TempDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite("");
        }
    }
}
