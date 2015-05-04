using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace Tomahawk.Models
{
    public class MyUser : IdentityUser
    {
        public virtual ICollection<Message> Messages { get; set; }
    }

    public class Message
    {
        public int ID { get; set; }
        [MinLength(1)]
        [MaxLength(140)]
        public string Text { get; set; }
        public virtual MyUser User { get; set; }
        public virtual ICollection<Reply> Replies { get; set; }
    }

    public class Reply
    {
        public int ID { get; set; }
        [MinLength(1)]
        [MaxLength(140)]
        public string Text { get; set; }
        public virtual MyUser User { get; set; }
        public virtual Message Parent { get; set; }
    }

    public class TomahawkContext : IdentityDbContext<MyUser>
    {
        public TomahawkContext() : base("DefaultConnection") { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Message> Messages { get; set; }
        public DbSet<Reply> Replies { get; set; }
    }
}