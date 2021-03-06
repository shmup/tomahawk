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
            string password = passwordHash.HashPassword("Password@123");

            var users = new List<MyUser>
            {
                new MyUser { UserName = "shmup", Email = "shmup@fake.com", PasswordHash = password, SecurityStamp = "fake" },
                new MyUser { UserName = "eric", Email = "eric@fake.com", PasswordHash = password, SecurityStamp = "fake" },
                new MyUser { UserName = "jed", Email = "jed@fake.com", PasswordHash = password, SecurityStamp = "fake" },
                new MyUser { UserName = "carmack", Email = "carmack@id.com", PasswordHash = password, SecurityStamp = "fake" }
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
                    Text = "Doom is a pretty cool game" ,
                    User = users.Single(u => u.UserName == "carmack"),
                },
                new Message {
                    Text = "Actually.." ,
                    User = users.Single(u => u.UserName == "jed"),
                },
                new Message {
                    Text = "turns out pager.js is really, really cool" ,
                    User = users.Single(u => u.UserName == "shmup"),
                },
                new Message {
                    Text = "I like gin and euchre" ,
                    User = users.Single(u => u.UserName == "jed"),
                },
                new Message {
                    Text = "I love playing euchre!" ,
                    User = users.Single(u => u.UserName == "eric"),
                },
                new Message {
                    Text = "VR is the future" ,
                    User = users.Single(u => u.UserName == "carmack"),
                },
            };

            messages.ForEach(m => context.Messages.Add(m));

            context.SaveChanges();

            foreach (Message m in messages)
            {
                var replies = new List<Reply>
                {
                    new Reply {
                        Text = "Fake Reply",
                        User = users.Single(u => u.UserName == "eric"),
                        Parent = m
                    },
                    new Reply {
                        Text = "Faker Reply",
                        User = users.Single(u => u.UserName == "jed"),
                        Parent = m
                    },
                    new Reply {
                        Text = "Fakest Reply",
                        User = users.Single(u => u.UserName == "carmack"),
                        Parent = m
                    }
                };

                replies.ForEach(r => context.Replies.Add(r));
                context.SaveChanges();
            };
        }
    }
}
