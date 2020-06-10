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

    public async Task<Like> GetLike(int userId, int recipientId)
    {
      return await _context.Likes.FirstOrDefaultAsync(u => u.LikerId == userId && u.LikeeId == recipientId);
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
      var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

      return user;
    }

    public async Task<PagedList<User>> GetUsers(UserParams userParams)
    {
      // get users from context without executing it at this time
      var users = _context.Users.OrderByDescending(u => u.LastActive).AsQueryable();

      users = users.Where(u => u.Id != userParams.UserId);
      users = users.Where(u => u.Gender == userParams.Gender);

      if (userParams.Likers)
      {
        var userLikers = await GetUserLikes(userParams.UserId, userParams.Likers);
        // check if id's from userlikes matches any users in users table
        users = users.Where(u => userLikers.Contains(u.Id));
      }

      if (userParams.Likees)
      {
        var userLikees = await GetUserLikes(userParams.UserId, userParams.Likers);
        // check if id's from userlikes matches any users in users table
        users = users.Where(u => userLikees.Contains(u.Id));
      }

      // know that user specified age that they are looking for
      if (userParams.MinAge != 18 || userParams.MaxAge != 99)
      {
        // calculate oldest birthday
        var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
        // calculate youngest birthday
        var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

        users = users.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
      }

      if (!string.IsNullOrEmpty(userParams.OrderBy))
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

    private async Task<IEnumerable<int>> GetUserLikes(int id, bool likers)
    {
      // get logged in user that includes likes and likees collection
      var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

      if (likers)
      {
        // return list of the likers of the currently logged in user
        return user.Likers.Where(u => u.LikeeId == id).Select(i => i.LikerId);
      }
      else
      {
        return user.Likees.Where(u => u.LikerId == id).Select(i => i.LikeeId);
      }
    }

    public async Task<bool> SaveAll()
    {
      // return true if more than 0 has been save to db
      return await _context.SaveChangesAsync() > 0;
    }

    public async Task<Message> GetMessage(int id)
    {
      return await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams)
    {
      var messages = _context.Messages.AsQueryable();

      switch (messageParams.MessageContainer)
      {
        case "Inbox":
          messages = messages.Where(u => u.RecipientId == messageParams.UserId && u.RecipientDeleted == false);
          break;
        case "Outbox":
          messages = messages.Where(u => u.SenderId == messageParams.UserId && u.SenderDeleted == false);
          break;
        default:
          messages = messages.Where(u => u.RecipientId == messageParams.UserId && u.RecipientDeleted == false && u.IsRead == false);
          break;
      }

      messages = messages.OrderByDescending(d => d.MessageSent);
      return await PagedList<Message>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);

    }

    public async Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId)
    {
      var messages = await _context.Messages
        .Where(m => m.RecipientId == userId && m.RecipientDeleted == false && m.SenderId == recipientId 
          || m.RecipientId == recipientId && m.SenderId == userId && m.SenderDeleted == false)
        .OrderByDescending(m => m.MessageSent).ToListAsync();

      return messages;

    }
  }
}