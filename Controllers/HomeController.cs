using Microsoft.AspNetCore.Mvc;
using Orb.Web.Services;

namespace Orb.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly BlogService _blogService;

        private readonly UserService _userService;
        
        public HomeController(BlogService blogService,UserService userService)
        {
            _blogService = blogService;
             _userService = userService;
        }
        
        public async Task<IActionResult> Index()
        {
            // Retrieve the 5 most recent published posts
            var posts = await _blogService.GetRecentPostsAsync(5);
            var users = await _userService.GetAllUsersAsync();
            IndexViewModel model = new IndexViewModel
            {
                Posts = posts,
                Users = users.ToList()
            };
            return View(model);
        }

        public async Task<IActionResult> LoadMorePosts(int page = 2, int pageSize = 5)
        {
            // Calculate the number of posts to skip (assuming page 1 was already loaded)
            var skipCount = (page - 1) * pageSize;
            var posts = await _blogService.GetRecentPostsAsync(int.MaxValue);
            var morePosts = posts.Skip(skipCount).Take(pageSize).ToList();

            return PartialView("_MorePosts", morePosts);
        }

        public IActionResult About()
        {
            return View();
        }
    }
}