using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment2.Models
{
    public class Photo
    {
        [Required]
        public int PhotoId
        {
            get;
            set;
        }

        [Required]
        [ForeignKey("BlogPostId")]
        public int BlogPostId
        {
            get;
            set;
        }

        [Required]
        [StringLength(255)]
        public string Filename
        {
            get;
            set;
        }
        [Required]
        [StringLength(2048)]
        public string Url
        {
            get;
            set;
        }
    }
}
