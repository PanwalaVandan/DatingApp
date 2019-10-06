using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;

namespace DatingApp.API.Data
{
    public interface IDatingRepository
    {
         void Add<T>(T entity) where T: class;

         void Delete<T>(T entity) where T: class;

         Task<PagedList<User>> GetUsers(UserParams userParams);

         Task<User> GetUser(int id);

         Task<bool> SaveAll();

         Task<Photo> GetPhoto(int id);

         Task<Photo> GetMainPhotoForUser(int userId);
        //  Method checks where like already exists for user
         Task<Like> GetLike(int userId, int reciptientId);
    }
}