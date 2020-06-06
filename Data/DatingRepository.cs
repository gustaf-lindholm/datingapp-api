using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
  public class DatingRepository : IDatingRepository
  {
    private readonly DataContext _context;
    public DatingRepository(DataContext context)
    {
      _context = context;
    }
    public void Add<T>(T entity) where T : class
    {
      _context.Add(entity);
    }

    public void Delete<T>(T entity) where T : class
    {
      _context.Remove(entity);
    }

    public async Task<Photo> GetMainPhotoForUser(int userId)
    {
      return await _context.Photos.Where(u => u.UserId == userId).FirstOrDefaultAsync(p => p.IsMain);
    }

    public async Task<Photo> GetPhoto(int id)
    {
      var photo = await _context.Photos.FirstOrDefaultAsync(p => p.id == id);

      return photo;
    }

    public async Task<User> GetUser(int id)
    {
      var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == id);

      return user;
    }

    public async Task<PagedList<User>> GetUsers(UserParams userParams)
    {
      // get users from context without executing it at this time
      var users = _context.Users.Include(p => p.Photos).OrderByDescending(u => u.LastActive).AsQueryable();
    
      users = users.Where(u => u.Id != userParams.UserId);
      users = users.Where(u => u.Gender == userParams.Gender);

      // know that user specified age that they are looking for
      if (userParams.MinAge != 18 || userParams.MaxAge != 99)
      {
          // calculate oldest birthday
          var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
          // calculate youngest birthday
          var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

          users = users.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
      }

      if(!string.IsNullOrEmpty(userParams.OrderBy))
      {
        switch (userParams.OrderBy)
        {
            case "created":
              users = users.OrderByDescending(u => u.Created);
              break;
            default:
              users = users.OrderByDescending(u => u.LastActive);
              break;
        }
      }

      // create and return new instance of PagedList
      return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
    }

    public async Task<bool> SaveAll()
    {
      // return true if more than 0 has been save to db
      return await _context.SaveChangesAsync() > 0;
    }
  }
}