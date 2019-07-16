using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment2.Models
{
    public class BadWord
    {
        [Required]
        public int BadWordId
        {
            get;
            set;
        }

        [Required]
        [StringLength(50)]
        public string Word
        {
            get;
            set;
        }
    }
}