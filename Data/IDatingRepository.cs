using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.models;

namespace DatingApp.API.Data
{
    public interface IDatingRepository
    {
        // "generic typeOf method". Add T (User|Photo) takes entity as parameter and
        // is constrained to just classes. T is typeOf class.
        // same method for both User and Photo
         void Add<T>(T entity) where T: class;
         void Delete<T>(T entity) where T: class;
         Task<bool> SaveAll();
         Task<IEnumerable<User>> GetUsers();
         Task<User> GetUser(int id);
    }
}