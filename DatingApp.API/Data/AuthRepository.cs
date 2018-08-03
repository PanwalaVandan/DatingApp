using System;
using System.Threading.Tasks;
//using DatingApp.API.Controllers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;//injecting Datacontext to the repo class 
        //injecting Datacontext to the repo class 
        public AuthRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<User> Login(string username, string password)
        {
            // returns collection of photos along with the users
            var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(x => x.Username == username);

            if(user == null){
                return null;
            }

            if(!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt)){
                return null;
            }
            //authentication is successfull
            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            //code for creating passwordSalt and passwordHash
            using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt)){
                //passwordSalt = hmac.Key;
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                for(int i = 0 ; i < computedHash.Length ; i++){
                    if(computedHash[i] != passwordHash[i]) return false;
                }
            }
            return true;
        }

        public async Task<User> Register(User user, string password)
        {
            byte [] passwordHash, passwordSalt;
            //out tells the compiler that the object will be initialized inside the function.
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            //code for creating passwordSalt and passwordHash
            using(var hmac = new System.Security.Cryptography.HMACSHA512()){
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> UserExists(string username)
        {
            if(await _context.Users.AnyAsync(x => x.Username == username))
            return true;

            return false;
        }
    }
}