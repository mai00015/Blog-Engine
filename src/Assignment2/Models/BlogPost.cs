using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment2.Models
{
    public class BlogPost
    {
        [Required]
        public int BlogPostId
        {
            get;
            set;
        }

        [Required]
        [ForeignKey("UserId")]
        public int UserId
        {
            get;
            set;
        }

        [Required]
        [StringLength(200)]
        public string Title
        {
            get;
            set;
        }
        [Required]
        [StringLength(400)]
        public string ShortDescription
        {
            get;
            set;
        }
        [Required]
        [StringLength(4000)]
        public string Content
        {
            get;
            set;
        }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Posted
        {
            get;
            set;
        }
        [Required]
        public bool IsAvailable
        {
            get;
            set;
        }
    }
}
