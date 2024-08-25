using Microsoft.EntityFrameworkCore;
using System;

namespace WT_API.Data
{
    
    public class Context : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
          optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=WT_API;Trusted_Connection=True;MultipleActiveResultSets=true");
        }

        public Context() { }

        public Context(DbContextOptions<Context> options) : base(options) { }
       
        public DbSet<Models.User> Users { get; set; }
        public DbSet<Models.Serial> Serials { get; set; }
        public DbSet<Models.Chapter> Chapters { get; set; }
        public DbSet<Models.Author> Authors { get; set; }
        public DbSet<Models.LikedSerial> LikedSerials { get; set; }
  }
}
