namespace Tomahawk.Migrations
{
    using System;
    using Tomahawk.Models;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Microsoft.AspNet.Identity;

    internal sealed class Configuration : DbMigrationsConfiguration<Tomahawk.Models.TomahawkContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Tomahawk.Models.TomahawkContext context)
        {
            var passwordHash = new PasswordHasher();
            string password = passwordHash.HashPassword("k3yB0ard!");

            var users = new List<MyUser>
            {
                new MyUser { UserName = "shmup", Email = "shmup@fake.com", PasswordHash = password },
                new MyUser { UserName = "eric", Email = "eric@fake.com", PasswordHash = password },
                new MyUser { UserName = "jed", Email = "jed@fake.com", PasswordHash = password },
                new MyUser { UserName = "burt", Email = "burt@fake.com", PasswordHash = password }
            };

            users.ForEach(u => context.Users.AddOrUpdate(m => m.Email, u));

            context.SaveChanges();

            var messages = new List<Message>
            {
                new Message {
                    Text = "Mayweather shouldn't have won" ,
                    User = users.Single(u => u.UserName == "shmup"),
                },
                new Message {
                    Text = "The Record Eagle could use some better JavaScript" ,
                    User = users.Single(u => u.UserName == "eric"),
                },
                new Message {
                    Text = "Euchre is not Eric's best card game" ,
                    User = users.Single(u => u.UserName == "jed"),
                },
                new Message {
                    Text = "Have you seen Ernie?" ,
                    User = users.Single(u => u.UserName == "burt"),
                },
            };

            messages.ForEach(m => context.Messages.Add(m));

            var replies = new List<Reply>
            {
            };
        }
    }
}
