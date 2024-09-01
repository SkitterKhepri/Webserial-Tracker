using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using WT_API.Models;

namespace WT_API.Data
{
    
    public class Context : IdentityDbContext<User>
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
          optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=WT_API;Trusted_Connection=True;MultipleActiveResultSets=true");
        }

        public Context() { }

        public Context(DbContextOptions<Context> options) : base(options) { }
       
        public DbSet<Models.Serial> Serials { get; set; }
        public DbSet<Models.Chapter> Chapters { get; set; }
        public DbSet<Models.Author> Authors { get; set; }
        public DbSet<Models.LikedSerial> LikedSerials { get; set; }
  }
}
