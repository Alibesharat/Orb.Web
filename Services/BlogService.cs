using Microsoft.EntityFrameworkCore;
using Orb.Web.Data;
using Orb.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orb.Web.Services
{
    public class BlogService
    {
        private readonly ApplicationDbContext _context;

        public BlogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<BlogPost>> GetAllPostsAsync()
        {
            return await _context.BlogPosts
                .Include(p => p.Author)
                .ToListAsync();
        }

        public async Task<List<BlogPost>> GetPublishedPostsAsync(string tag = null, string sort = "latest")
        {
            var query = _context.BlogPosts
                .Include(p => p.Author)
                .Where(p => p.IsPublished);

            if (!string.IsNullOrEmpty(tag))
            {
                query = query.Where(p => p.Tags.Contains(tag));
            }

            query = sort switch
            {
                "popular" => query.OrderByDescending(p => p.ViewCount),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            return await query.ToListAsync();
        }

        public async Task<BlogPost> GetPostByIdAsync(int id)
        {
            return await _context.BlogPosts
                .Include(p => p.Author)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<int> AddPostAsync(BlogPost post)
        {
            _context.BlogPosts.Add(post);
            await _context.SaveChangesAsync();
            return post.Id;
        }

        public async Task UpdatePostAsync(BlogPost post)
        {
            _context.Entry(post).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeletePostAsync(int id)
        {
            var post = await _context.BlogPosts.FindAsync(id);
            if (post != null)
            {
                _context.BlogPosts.Remove(post);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<string>> GetPopularTagsAsync(int count)
        {
            var posts = await _context.BlogPosts
                .Where(p => p.IsPublished && p.Tags != null && p.Tags.Any())
                .ToListAsync();

            var allTags = new List<string>();
            foreach (var post in posts)
            {
                allTags.AddRange(post.Tags);
            }

            return allTags
                .GroupBy(t => t)
                .Select(g => new { Tag = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(count)
                .Select(x => x.Tag)
                .ToList();
        }

        public async Task IncrementViewCountAsync(int postId)
        {
            var post = await _context.BlogPosts.FindAsync(postId);
            if (post != null)
            {
                post.ViewCount++;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<BlogPost>> GetRelatedPostsAsync(int postId, int count)
        {
            var post = await _context.BlogPosts
                .Include(p => p.Author)
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null || post.Tags == null || !post.Tags.Any())
            {
                return await _context.BlogPosts
                    .Where(p => p.IsPublished && p.Id != postId)
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(count)
                    .Include(p => p.Author)
                    .ToListAsync();
            }

            var relatedPosts = await _context.BlogPosts
                .Where(p => p.IsPublished && p.Id != postId)
                .ToListAsync();

            return relatedPosts
                .Where(p => p.Tags != null && p.Tags.Any(t => post.Tags.Contains(t)))
                .OrderByDescending(p => p.Tags.Count(t => post.Tags.Contains(t)))
                .ThenByDescending(p => p.CreatedAt)
                .Take(count)
                .ToList();
        }

        public async Task<List<BlogPost>> GetUserPostsAsync(int userId)
        {
            return await _context.BlogPosts
                .Where(p => p.AuthorId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<BlogPost>> GetRecentPostsAsync(int count)
        {
            return await _context.BlogPosts
            .Include(c => c.Author)
                .Where(p => p.IsPublished)
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .ToListAsync();
        }
    }
}