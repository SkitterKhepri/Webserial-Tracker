using Microsoft.EntityFrameworkCore;
using System;

namespace WT_API.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }
       
        public DbSet<Models.User> Users { get; set; }
        public DbSet<Models.Serial> Serials { get; set; }
        public DbSet<Models.Book> Books { get; set; }
        public DbSet<Models.Chapter> Chapters { get; set; }
        public DbSet<Models.LikedSerial> LikedSerials { get; set; }
    }
}
