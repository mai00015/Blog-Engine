using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment2.Models
{
    public class CommentDetail
    {
        //public User user { get; set; }
        public Comment comment { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
    }
}
