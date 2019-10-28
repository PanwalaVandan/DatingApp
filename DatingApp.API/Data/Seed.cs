using System.Collections.Generic;
using System.Linq;
using DatingApp.API.Models;
using Newtonsoft.Json;

namespace DatingApp.API.Data
{
    public class Seed
    {
        // private readonly DataContext _context;
        // public Seed(DataContext context)
        // {
        //     _context = context;
        // }

        public static void SeedUsers(DataContext context)
        {
            // _context.Users.RemoveRange(_context.Users);
            // _context.SaveChanges();

            //Seed Users
            if(!context.Users.Any()) {
            var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
            var users = JsonConvert.DeserializeObject<List<User>>(userData);

            foreach(var user in users)
            {
                //create the password hash
                byte[] passwordHash, passwordsalt;
                CreatePasswordHash("password", out passwordHash, out passwordsalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordsalt;
                user.Username = user.Username.ToLower();
                
                context.Users.Add(user);

            }
            //Adds the Data to the Database
            context.SaveChanges();
            }
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            //code for creating passwordSalt and passwordHash
            using(var hmac = new System.Security.Cryptography.HMACSHA512()){
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}