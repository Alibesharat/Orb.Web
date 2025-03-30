using System.ComponentModel.DataAnnotations;

namespace Orb.Web.Models
{
    public class PostCreateViewModel
    {
        [Required(ErrorMessage = "عنوان مقاله الزامی است")]
        [StringLength(100, ErrorMessage = "عنوان نباید بیشتر از {1} کاراکتر باشد")]
        public string Title { get; set; }
        
        [Required(ErrorMessage = "محتوای مقاله الزامی است")]
        public string Content { get; set; }
        
        public string Summary { get; set; }
        
        public string CoverImageUrl { get; set; }
        
        public string Tags { get; set; }
        
        public bool PublishImmediately { get; set; } = true;
    }
}