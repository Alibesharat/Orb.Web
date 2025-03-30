using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orb.Web.Models;
using Orb.Web.Services;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Orb.Web.Controllers
{
    public class BlogController : Controller
    {
        private readonly BlogService _blogService;
        private readonly UserService _userService;

        public BlogController(BlogService blogService, UserService userService)
        {
            _blogService = blogService;
            _userService = userService;
        }

        public async Task<IActionResult> Index(string tag = null, string sort = "latest")
        {
            var posts = await _blogService.GetPublishedPostsAsync(tag, sort);
            ViewBag.CurrentTag = tag;
            ViewBag.CurrentSort = sort;
            ViewBag.PopularTags = await _blogService.GetPopularTagsAsync(10);
            return View(posts);
        }

        public async Task<IActionResult> Post(int id)
        {
            var post = await _blogService.GetPostByIdAsync(id);
            if (post == null || !post.IsPublished)
            {
                return NotFound();
            }
            
            // Increment view count
            await _blogService.IncrementViewCountAsync(id);
            
            ViewBag.RelatedPosts = await _blogService.GetRelatedPostsAsync(id, 3);
            return View(post);
        }
        
        [Authorize]
        public IActionResult New()
        {
            return View(new PostCreateViewModel());
        }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> New(PostCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                
                var post = new BlogPost
                {
                    Title = model.Title,
                    Content = model.Content,
                    Summary = model.Summary ?? model.Content.Substring(0, Math.Min(200, model.Content.Length)) + "...",
                    CoverImageUrl = model.CoverImageUrl,
                    CreatedAt = DateTime.UtcNow,
                    IsPublished = model.PublishImmediately,
                    AuthorId = userId,
                    Tags = model.Tags?.Split(',').Select(t => t.Trim()).ToList(),
                    ReadTime = CalculateReadTime(model.Content)
                };
                
                var postId = await _blogService.AddPostAsync(post);
                
                if (model.PublishImmediately)
                {
                    TempData["Success"] = "مقاله شما با موفقیت منتشر شد!";
                    return RedirectToAction("Post", new { id = postId });
                }
                else
                {
                    TempData["Success"] = "پیش‌نویس مقاله شما ذخیره شد.";
                    return RedirectToAction("Edit", new { id = postId });
                }
            }
            
            return View(model);
        }
        
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _blogService.GetPostByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (post.AuthorId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }
            
            var viewModel = new PostCreateViewModel
            {
                Title = post.Title,
                Content = post.Content,
                Summary = post.Summary,
                CoverImageUrl = post.CoverImageUrl,
                Tags = post.Tags != null ? string.Join(", ", post.Tags) : "",
                PublishImmediately = post.IsPublished
            };
            
            return View(viewModel);
        }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, PostCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var post = await _blogService.GetPostByIdAsync(id);
                if (post == null)
                {
                    return NotFound();
                }
                
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (post.AuthorId != userId && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }
                
                post.Title = model.Title;
                post.Content = model.Content;
                post.Summary = model.Summary ?? model.Content.Substring(0, Math.Min(200, model.Content.Length)) + "...";
                post.CoverImageUrl = model.CoverImageUrl;
                post.UpdatedAt = DateTime.UtcNow;
                post.IsPublished = model.PublishImmediately;
                post.Tags = model.Tags?.Split(',').Select(t => t.Trim()).ToList();
                post.ReadTime = CalculateReadTime(model.Content);
                
                await _blogService.UpdatePostAsync(post);
                
                TempData["Success"] = model.PublishImmediately ? "مقاله با موفقیت به‌روزرسانی شد." : "پیش‌نویس با موفقیت ذخیره شد.";
                
                if (model.PublishImmediately)
                {
                    return RedirectToAction("Post", new { id });
                }
            }
            
            return View(model);
        }
        
        [Authorize]
        public async Task<IActionResult> MyPosts()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var posts = await _blogService.GetUserPostsAsync(userId);
            return View(posts);
        }
        
        private int CalculateReadTime(string content)
        {
            // Average reading speed: 200 words per minute
            var wordCount = content.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
            return Math.Max(1, (int)Math.Ceiling(wordCount / 200.0));
        }
    }
}