
using DatingApp.API.models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
  public class DataContext : DbContext
  {
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    // Values is the name of the database table
    public DbSet<Value> Values { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<Photo> Photos { get; set; }

    public DbSet<Like> Likes { get; set; }

    // override default db table creation
    protected override void OnModelCreating(ModelBuilder builder)
    {
      // define primary key
      builder.Entity<Like>()
          .HasKey(k => new { k.LikerId, k.LikeeId });

      // relationship
      builder.Entity<Like>()
          .HasOne(u => u.Likee)
          .WithMany(u => u.Likers)
          .HasForeignKey(u => u.LikeeId)
          .OnDelete(DeleteBehavior.Restrict);

      builder.Entity<Like>()
          .HasOne(u => u.Liker)
          .WithMany(u => u.Likees)
          .HasForeignKey(u => u.LikerId)
          .OnDelete(DeleteBehavior.Restrict);
    }

  }
}