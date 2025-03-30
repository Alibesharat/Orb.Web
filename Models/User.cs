using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Orb.Web.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "نام کاربری الزامی است")]
        [StringLength(30, ErrorMessage = "نام کاربری باید بین {2} و {1} کاراکتر باشد", MinimumLength = 3)]
        public string Username { get; set; }
        
        [Required(ErrorMessage = "ایمیل الزامی است")]
        [EmailAddress(ErrorMessage = "ایمیل نامعتبر است")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "رمز عبور الزامی است")]
        public string Password { get; set; }
        
        public string DisplayName { get; set; }
        
        [StringLength(160, ErrorMessage = "توضیحات نباید بیشتر از {1} کاراکتر باشد")]
        public string Bio { get; set; }
        
        public string ProfileImageUrl { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public List<BlogPost> Posts { get; set; }
        
        public List<User> Following { get; set; }
        
        public List<User> Followers { get; set; }
    }
}