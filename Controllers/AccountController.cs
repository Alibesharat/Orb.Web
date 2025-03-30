using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Orb.Web.Models;
using Orb.Web.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Orb.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserService _userService;

        public AccountController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // Log the incoming request
            Console.WriteLine($"Register POST received for user: {model.Username}");
            
            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState invalid:");
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"- {state.Key}: {error.ErrorMessage}");
                    }
                }
                return View(model);
            }
            
            // Check if username is already taken
            if (await _userService.UsernameExistsAsync(model.Username))
            {
                ModelState.AddModelError("Username", "نام کاربری قبلاً استفاده شده است");
                return View(model);
            }

            // Check if email is already taken
            if (await _userService.EmailExistsAsync(model.Email))
            {
                ModelState.AddModelError("Email", "این ایمیل قبلاً ثبت شده است");
                return View(model);
            }

            // Create new user
            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                DisplayName = model.DisplayName,
                Password = _userService.HashPassword(model.Password),
                Bio = "",
                ProfileImageUrl = $"https://randomuser.me/api/portraits/{(new Random().Next(0, 2) == 0 ? "men" : "women")}/{new Random().Next(1, 99)}.jpg",
                CreatedAt = DateTime.UtcNow
            };

            await _userService.CreateUserAsync(user);

            // Auto login after registration
            await SignInAsync(user);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.AuthenticateUserAsync(model.Username, model.Password);
                if (user != null)
                {
                    await SignInAsync(user);
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "نام کاربری یا رمز عبور اشتباه است");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        private async Task SignInAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("DisplayName", user.DisplayName ?? "")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }
    }
}