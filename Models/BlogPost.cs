using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Orb.Web.Models
{
    public class BlogPost
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "عنوان مقاله الزامی است")]
        [StringLength(100, ErrorMessage = "عنوان نباید بیشتر از {1} کاراکتر باشد")]
        public string Title { get; set; }
        
        [Required(ErrorMessage = "محتوای مقاله الزامی است")]
        public string Content { get; set; }
        
        public string Summary { get; set; }
        
        public string CoverImageUrl { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
        
        public bool IsPublished { get; set; }
        
        public int ReadTime { get; set; }
        
        public int AuthorId { get; set; }
        
        public User Author { get; set; }
        
        public List<string> Tags { get; set; }
        
        public int ViewCount { get; set; }
        
        public int LikeCount { get; set; }
    }
}