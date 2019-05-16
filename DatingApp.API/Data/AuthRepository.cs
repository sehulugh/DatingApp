using System;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;
        }
        #region LOGIN
        public async Task<User> Login(string username, string password)
        {
            //Check to see if user exists
            var user = await _context.Users.FirstOrDefaultAsync(x=>x.Username == username);
                if (user==null)
                    return null;

            //if usename exists then verify password
                if (!VerifyPasswordHash( password ,user.PasswordHash,user.PasswordSalt))
                return null;

                return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {                
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0;i<computedHash.Length;i++)
                {
                    if (computedHash[i]!=passwordHash[i])
                    return false;
                }                  
            }
            return true;

        }   
        #endregion

        #region REGISTER
        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordhash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
            
        }

        private void CreatePasswordhash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            }
            
        }
#endregion

        public async Task<bool> UserExists(string username)
        {
           
            if (await _context.Users.AnyAsync(x => x.Username == username))
           
                return  false;
            //else
                return true;

        }
    }
}