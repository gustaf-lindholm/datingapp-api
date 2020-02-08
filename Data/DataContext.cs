
using DatingApp.API.models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options){}

        // Values is the name of the database table
        public DbSet<Value> Values {get; set;}
    }
}