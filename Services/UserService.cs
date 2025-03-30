using Microsoft.EntityFrameworkCore;
using Orb.Web.Data;
using Orb.Web.Models;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Orb.Web.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await GetUserByIdAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public async Task<User> AuthenticateUserAsync(string username, string password)
        {
            string hashedPassword = HashPassword(password);
            
            return await _context.Users.FirstOrDefaultAsync(u => 
                (u.Username == username || u.Email == username) && 
                u.Password == hashedPassword);
        }

        public async Task<int> GetFollowerCountAsync(int userId)
        {
            return await _context.Users.CountAsync(u => u.Following.Any(f => f.Id == userId));
        }

        public async Task<int> GetFollowingCountAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Following)
                .FirstOrDefaultAsync(u => u.Id == userId);
                
            return user?.Following?.Count ?? 0;
        }

        public async Task<bool> IsFollowingAsync(int followerId, int followedId)
        {
            var follower = await _context.Users
                .Include(u => u.Following)
                .FirstOrDefaultAsync(u => u.Id == followerId);
                
            return follower?.Following?.Any(f => f.Id == followedId) ?? false;
        }

        public async Task FollowUserAsync(int followerId, int followedId)
        {
            var follower = await _context.Users
                .Include(u => u.Following)
                .FirstOrDefaultAsync(u => u.Id == followerId);
                
            var followed = await _context.Users.FindAsync(followedId);
            
            if (follower != null && followed != null && follower.Id != followed.Id)
            {
                if (follower.Following == null)
                    follower.Following = new List<User>();
                
                if (!follower.Following.Contains(followed))
                {
                    follower.Following.Add(followed);
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task UnfollowUserAsync(int followerId, int followedId)
        {
            var follower = await _context.Users
                .Include(u => u.Following)
                .FirstOrDefaultAsync(u => u.Id == followerId);
                
            var followed = await _context.Users.FindAsync(followedId);
            
            if (follower != null && followed != null && follower.Following?.Contains(followed) == true)
            {
                follower.Following.Remove(followed);
                await _context.SaveChangesAsync();
            }
        }
    }
}