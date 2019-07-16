using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment2.Models
{
    public class Assignment2DataContext : DbContext
    {
        public Assignment2DataContext(DbContextOptions<Assignment2DataContext> options)
            : base(options)
        {

        }


        /*BlogPost*/
        public DbSet<BlogPost> BlogPosts { get; set; }

        /*Comment*/
        public DbSet<Comment> Comments { get; set; }

        /*Role*/
        public DbSet<Role> Roles { get; set; }

        /*User*/
        public DbSet<User> Users { get; set; }
        public DbSet<BadWord> BadWords { get; set; }

        public DbSet<Photo> Photos { get; set; }

    }
}
