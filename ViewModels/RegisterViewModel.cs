using System.ComponentModel.DataAnnotations;

public class RegisterViewModel
    {
        [Required(ErrorMessage = "نام کاربری الزامی است")]
        [StringLength(30, ErrorMessage = "نام کاربری باید بین {2} و {1} کاراکتر باشد", MinimumLength = 3)]
        public string Username { get; set; }
        
        [Required(ErrorMessage = "ایمیل الزامی است")]
        [EmailAddress(ErrorMessage = "ایمیل نامعتبر است")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "نام نمایشی الزامی است")]
        [StringLength(50, ErrorMessage = "نام نمایشی نباید بیشتر از {1} کاراکتر باشد")]
        public string DisplayName { get; set; }
        
        [Required(ErrorMessage = "رمز عبور الزامی است")]
        [StringLength(100, ErrorMessage = "رمز عبور باید حداقل {2} کاراکتر باشد", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "رمز عبور و تکرار آن مطابقت ندارند")]
        public string ConfirmPassword { get; set; }
    }
    