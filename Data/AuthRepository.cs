using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Api.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;

        public AuthRepository(DataContext context)
        {
            this._context = context;
        }

        public async Task<User> Login(string userName, string password)
        {
            var user = await this._context.Users.Include(p => p.Photos).FirstOrDefaultAsync(x => x.UserName == userName);
            if (user == null)
                return null;

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

     
        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash;
            byte[] passworSalt;
            CreatePasswordHass(password, out passwordHash, out passworSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passworSalt;

            await this._context.Users.AddAsync(user);
            await this._context.SaveChangesAsync();

            return user;
        }

        public async Task<bool> UserExist(string username)
        {
            if (await this._context.Users.AnyAsync(x => x.UserName == username))
                return true;

            return false;
        }

        #region Métodos Privados
        private void CreatePasswordHass(string password, out byte[] passwordHash, out byte[] passworSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passworSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for(int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i]) return false;
                }
            }

            return true;
        }

        #endregion
    }
}
