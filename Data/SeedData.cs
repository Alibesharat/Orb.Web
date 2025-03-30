using System;
using System.Linq;
using System.Collections.Generic;
using Orb.Web.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Orb.Web.Data
{
    public static class SeedData
    {
        public static void EnsureSeedData(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                context.Database.EnsureCreated();

                // Seed Users if none exist
               
                    var users = new List<User>
                    {
                        new User { Username = "alirezai", Email = "alirezai@example.com", Password = "password1", DisplayName = "علی رضایی", Bio = "بیوگرافی علی رضایی", ProfileImageUrl = "https://randomuser.me/api/portraits/men/1.jpg", CreatedAt = DateTime.UtcNow },
                        new User { Username = "zahrahasini", Email = "zahrahasini@example.com", Password = "password2", DisplayName = "زهرا حسینی", Bio = "بیوگرافی زهرا حسینی", ProfileImageUrl = "https://randomuser.me/api/portraits/women/2.jpg", CreatedAt = DateTime.UtcNow },
                        new User { Username = "amirmansouri", Email = "amirmansouri@example.com", Password = "password3", DisplayName = "امیر منصوری", Bio = "بیوگرافی امیر منصوری", ProfileImageUrl = "https://randomuser.me/api/portraits/men/3.jpg", CreatedAt = DateTime.UtcNow },
                        new User { Username = "minaaekbari", Email = "minaaekbari@example.com", Password = "password4", DisplayName = "مینا اکبری", Bio = "بیوگرافی مینا اکبری", ProfileImageUrl = "https://randomuser.me/api/portraits/women/4.jpg", CreatedAt = DateTime.UtcNow },
                        new User { Username = "hosseinmohammadi", Email = "hosseinmohammadi@example.com", Password = "password5", DisplayName = "حسین محمدی", Bio = "بیوگرافی حسین محمدی", ProfileImageUrl = "https://randomuser.me/api/portraits/men/5.jpg", CreatedAt = DateTime.UtcNow }
                    };

                    context.Users.AddRange(users);
                    context.SaveChanges();
                

                // Seed Blog Posts if none exist
                
                    // For simplicity, use the first user as the author for all posts
                    var firstUser = context.Users.First();

                    var posts = new List<BlogPost>
                    {
                        new BlogPost
                        {
                            Title = "مقاله اول",
                            Content = "<p>این محتوای مقاله اول است.</p>",
                            Summary = "خلاصه مقاله اول",
                            CoverImageUrl = "https://picsum.photos/id/26/1200/600",
                            CreatedAt = DateTime.UtcNow,
                            IsPublished = true,
                            AuthorId = firstUser.Id,
                            Tags = new List<string>{ "تکنولوژی", "خبر" },
                            ReadTime = 3,
                            ViewCount = 0,
                            LikeCount = 0
                        },
                        new BlogPost
                        {
                            Title = "مقاله دوم",
                            Content = "<p>این محتوای مقاله دوم است.</p>",
                            Summary = "خلاصه مقاله دوم",
                            CoverImageUrl = "https://picsum.photos/id/27/1200/600",
                            CreatedAt = DateTime.UtcNow,
                            IsPublished = true,
                            AuthorId = firstUser.Id,
                            Tags = new List<string>{ "طراحی", "هنر" },
                            ReadTime = 4,
                            ViewCount = 0,
                            LikeCount = 0
                        },
                        new BlogPost
                        {
                            Title = "مقاله سوم",
                            Content = "<p>این محتوای مقاله سوم است.</p>",
                            Summary = "خلاصه مقاله سوم",
                            CoverImageUrl = "https://picsum.photos/id/28/1200/600",
                            CreatedAt = DateTime.UtcNow,
                            IsPublished = true,
                            AuthorId = firstUser.Id,
                            Tags = new List<string>{ "کسب و کار", "روند" },
                            ReadTime = 5,
                            ViewCount = 0,
                            LikeCount = 0
                        },
                        new BlogPost
                        {
                            Title = "مقاله چهارم",
                            Content = "<p>این محتوای مقاله چهارم است.</p>",
                            Summary = "خلاصه مقاله چهارم",
                            CoverImageUrl = "https://picsum.photos/id/29/1200/600",
                            CreatedAt = DateTime.UtcNow,
                            IsPublished = true,
                            AuthorId = firstUser.Id,
                            Tags = new List<string>{ "سبک زندگی", "سلامت" },
                            ReadTime = 3,
                            ViewCount = 0,
                            LikeCount = 0
                        },
                        new BlogPost
                        {
                            Title = "مقاله پنجم",
                            Content = "<p>این محتوای مقاله پنجم است.</p>",
                            Summary = "خلاصه مقاله پنجم",
                            CoverImageUrl = "https://picsum.photos/id/30/1200/600",
                            CreatedAt = DateTime.UtcNow,
                            IsPublished = true,
                            AuthorId = firstUser.Id,
                            Tags = new List<string>{ "سفر", "ماجراجویی" },
                            ReadTime = 4,
                            ViewCount = 0,
                            LikeCount = 0
                        }
                    };

                    context.BlogPosts.AddRange(posts);
                    context.SaveChanges();
                }
            }
        }
    }
