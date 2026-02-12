using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ONnegha_Mefisto_Theatre_Company.Models
{
    public class DatabaseInitialiser : DropCreateDatabaseAlways<TheatreDbContext>
    {
        protected override void Seed(TheatreDbContext context)
        {
            base.Seed(context);

            //if there are no records in the Users table, add some default users
            if (!context.Users.Any())
            {
                //first we create some roles and store them in the roles table


                //to create and store roles we need a role manager
                RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

                //if the admin role doesnt exist, create it
                //admins have full access to the system, they can manage users, roles, and all other aspects of the system
                //admins also have the authority to promote other users to admin status, they can also demote other admins to non-admin status, they have the highest level of access and control over the system
                if (!roleManager.RoleExists("Admin"))
                {
                    roleManager.Create(new IdentityRole("Admin"));
                }

                //if the editor role doesnt exist, create it
                //editors have access to manage content, they can create, edit, and delete content but they cannot manage users or roles
                if (!roleManager.RoleExists("Editor"))
                {
                    roleManager.Create(new IdentityRole("Editor"));
                }

                //if the moderator role doesnt exist, create it
                //moderators have access to manage comments, they can view content and they can also manage comments by deleting inappropriate comments, they cannot create or edit content and can suspend users
                if (!roleManager.RoleExists("Moderator"))
                {
                    roleManager.Create(new IdentityRole("Moderator"));
                }

                //if the user role doesnt exist, create it
                //users have access to view content, they can view content but they cannot comment or interact with content in any way
                if (!roleManager.RoleExists("User"))
                {
                    roleManager.Create(new IdentityRole("User"));
                }

                //if the member role doesnt exist, create it
                //members have access to view content and comment on content, they can view content and they can also comment on content
                if (!roleManager.RoleExists("Member"))
                {
                    roleManager.Create(new IdentityRole("Member"));
                }
                context.SaveChanges();


                //******************************************************************************************************************************
                //now we create some users and store them in the users table

                //to create and store users we need a user manager
                UserManager<User> userManager = new UserManager<User>(new UserStore<User>(context));

                //create an admin user
                //check if the admin user already exists, if not create it and assign it to the admin role
                if (userManager.FindByName("admin@MTC.com") == null)
                {
                    // Super liberal password validation for password for seeds
                    userManager.PasswordValidator = new PasswordValidator
                    {
                        RequireDigit = false,
                        RequiredLength = 1,
                        RequireLowercase = false,
                        RequireNonLetterOrDigit = false,
                        RequireUppercase = false,
                    };

                    //create an admin employee
                    var administrator = new Staff
                    {
                        UserName = "admin@MTC.com",
                        Email = "admin@MTC.com",
                        FirstName = "Adam",
                        LastName = "Adamson",
                        Street = "12 Greedy St",
                        City = "Glasgow",
                        PostCode = "G5 7GG",
                        RegisteredAt = DateTime.Now.AddYears(-5),
                        EmailConfirmed = true
                    };
                    //add admin to the Users table
                    userManager.Create(administrator, "admin123");
                    //assign it to the admin role
                    userManager.AddToRole(administrator.Id, "Admin");
                }

                //create some editor users
                //check if the editor user already exists, if not create it and assign it to the editor role
                var jeff = new Staff
                {
                    UserName = "editor1@MTC.com",
                    Email = "editor1@MTC.com",
                    FirstName = "Jeff",
                    LastName = "Jefferson",
                    Street = "156 Gladys St",
                    City = "Glasgow",
                    PostCode = "G9 7GG",
                    RegisteredAt = DateTime.Now,
                    EmailConfirmed = true,
                    PhoneNumber = "07355829778",
                    isPromoted = false
                };
                if (userManager.FindByName("editor1@MTC.com") == null)
                {
                    //add jeff and the password to the Users table
                    userManager.Create(jeff, "editor123");
                    //assign the editor role to jeff
                    userManager.AddToRole(jeff.Id, "Editor");
                }

                var jane = new Staff
                {
                    UserName = "editor2@MTC.com",
                    Email = "editor2@MTC.com",
                    FirstName = "Jane",
                    LastName = "Jenkins",
                    Street = "6 Elmo St",
                    City = "Glasgow",
                    PostCode = "G1 7UM",
                    RegisteredAt = DateTime.Now,
                    EmailConfirmed = true,
                    PhoneNumber = "07355856098",
                    isPromoted = false
                };
                if (userManager.FindByName("editor2@MTC.com") == null)
                {
                    //add jane and the password to the Users table
                    userManager.Create(jane, "editor456");
                    //assign the editor role to jane
                    userManager.AddToRole(jane.Id, "Editor");
                }

                //create a moderator user
                //check if the moderator user already exists, if not create it and assign it to the moderator role
                var michael = new Staff
                {
                    UserName = "moderator@MTC.com",
                    Email = "moderator@MTC.com",
                    FirstName = "Michael",
                    LastName = "Phelps",
                    Street = "34 Olympic St",
                    City = "Glasgow",
                    PostCode = "G2 7GG",
                    RegisteredAt = DateTime.Now,
                    EmailConfirmed = true,
                    PhoneNumber = "07355812345",
                    isPromoted = false
                };
                if (userManager.FindByName("moderator@MTC.com") == null)
                {
                    //add michael and the password to the Users table
                    userManager.Create(michael, "moderator123");
                    //assign the moderator role to michael
                    userManager.AddToRole(michael.Id, "Moderator");
                }

                //create some member users
                //check if the member user already exists, if not create it and assign it to the member role
                var bill = new Member
                {
                    UserName = "bill@gmail.com",
                    Email = "bill@gmail.com",
                    FirstName = "Billy",
                    LastName = "Crow",
                    Street = "123 Happy St",
                    City = "Glasgow",
                    PostCode = "G56 7DF",
                    IsSuspended = false,
                    RegisteredAt = DateTime.Now,
                    dateRegisteredAsMember = DateTime.Now,
                    EmailConfirmed = true,
                    PhoneNumber = "07563902144"
                };
                if (userManager.FindByName("bill@gmail.com") == null)
                {
                    userManager.Create(bill, "member1");
                    userManager.AddToRole(bill.Id, "Member");
                }

                var steve = new Member
                {
                    UserName = "Steve",
                    Email = "steveb@gmail.com",
                    EmailConfirmed = true,
                    FirstName = "Steve",
                    LastName = "Fist",
                    Street = "1 Confused St",
                    City = "Edinburgh",
                    PostCode = "EH6 7DF",
                    RegisteredAt = DateTime.Now.AddMonths(-10),
                    dateRegisteredAsMember = DateTime.Now,
                    PhoneNumber = "07988441227"
                };
                if (userManager.FindByName("steveb@gmail.com") == null)
                {
                    userManager.Create(steve, "member3");
                    userManager.AddToRoles(steve.Id, "Member");
                }

                //a suspended member user
                var bob = new Member
                {
                    UserName = "bob@gmail.com",
                    Email = "bob@gmail.com",
                    FirstName = "Bob",
                    LastName = "Williams",
                    Street = "56 Grumpy St",
                    City = "Glasgow",
                    PostCode = "G56 7DF",
                    IsSuspended = true,
                    RegisteredAt = DateTime.Now.AddYears(-1),
                    dateRegisteredAsMember = DateTime.Now.AddMonths(-6),
                    EmailConfirmed = true,
                    PhoneNumber = "07421558903"
                };
                if (userManager.FindByName("bob@gmail.com") == null)
                {
                    userManager.Create(bob, "member2");
                    userManager.AddToRoles(bob.Id, "Member");
                }

                //create some regular users
                var gary = new User
                {
                    UserName = "gary@gmail.com",
                    Email = "gary@gmail.com",
                    EmailConfirmed = true,
                    FirstName = "Garry",
                    LastName = "Hugh",
                    Street = "3 Hungry St",
                    City = "Glasgow",
                    PostCode = "G6 7DF",
                    RegisteredAt = DateTime.Now,
                    PhoneNumber = "07988441227"

                };
                if (userManager.FindByName("gary@gmail.com") == null)
                {
                    userManager.Create(gary, "user2");
                    userManager.AddToRoles(gary.Id, "User");
                }

                //save the changes to the database
                context.SaveChanges();

                //******************************************************************************************************************************
                //now we can create some categories

                //create some categories
                var news = new Category() { Name = "News" };
                var events = new Category() { Name = "Events" };
                var reviews = new Category() { Name = "Reviews" };
                var interviews = new Category() { Name = "Interviews" };
                var cars = new Category() { Name = "Cars" };
                var bts = new Category() { Name = "Behind the Scenes" };

                if (!context.Categories.Any())
                {
                    //add each category to the Categories table
                    context.Categories.Add(news);
                    context.Categories.Add(events);
                    context.Categories.Add(reviews);
                    context.Categories.Add(interviews);
                    context.Categories.Add(cars);
                    context.Categories.Add(bts);
                }



                //save the changes to the database
                context.SaveChanges();

                //******************************************************************************************************************************
                //now we can create some posts

                //create a few posts
                var post1 = new Post()
                {
                    Title = "New Play Announced",
                    Content = "We are excited to announce our new play, 'The Great Gatsby', which will be opening next month. Stay tuned for more details!",
                    DatePosted = DateTime.Now.AddDays(-10),
                    Staff = context.Users.OfType<Staff>().First(u => u.UserName == "editor1@MTC.com"),
                    Category = news
                };
                var post2 = new Post()
                {
                    Title = "Upcoming Events",
                    Content = "Join us for our upcoming events, including a special performance of 'Romeo and Juliet' and a behind-the-scenes tour of our theatre.",
                    DatePosted = DateTime.Now.AddDays(-5),
                    Staff = jane,
                    Category = events
                };
                var post3 = new Post()
                {
                    Title = "Play Review: 'Hamlet'",
                    Content = "Our recent production of 'Hamlet' received rave reviews from critics and audiences alike. Read our full review to find out why it was such a hit!",
                    DatePosted = DateTime.Now.AddDays(-2),
                    Staff = jeff,
                    Category = reviews
                };
                var post4 = new Post()
                {
                    Title = "Interview with the Director",
                    Content = "We sat down with our director, Jane Jenkins, to discuss her vision for our upcoming production of 'Macbeth' and what audiences can expect.",
                    DatePosted = DateTime.Now.AddDays(-1),
                    Staff = jane,
                    Category = interviews
                };

                if (!context.Posts.Any())
                {
                    //add each post to the Posts table
                    context.Posts.Add(post1);
                    context.Posts.Add(post2);
                    context.Posts.Add(post3);
                    context.Posts.Add(post4);
                }



                //save the changes to the database
                context.SaveChanges();

                //******************************************************************************************************************************
                //now we can create some comments

                //create some comments
                var comment1 = new Comment()
                {
                    Content = "I can't wait to see 'The Great Gatsby'! It sounds amazing!",
                    dateCommented = DateTime.Now.AddDays(-9),
                    Post = post1,
                    Member = bill,
                    isApproved = true
                };
                var comment2 = new Comment()
                {
                    Content = "The behind-the-scenes tour sounds like a lot of fun! I hope to attend.",
                    dateCommented = DateTime.Now.AddDays(-4),
                    Post = post2,
                    Member = bill,
                    isApproved = true
                };
                var comment3 = new Comment()
                {
                    Content = "I loved the review of 'Hamlet'! It was very insightful and made me want to see the play even more.",
                    dateCommented = DateTime.Now.AddDays(-1),
                    Post = post3,
                    Member = bill,
                    isApproved = true
                };
                var comment4 = new Comment()
                {
                    Content = "The interview with the director was fascinating! I can't wait to see her vision for 'Macbeth' come to life on stage.",
                    dateCommented = DateTime.Now,
                    Post = post4,
                    Member = bill,
                    isApproved = true
                };
                var comment5 = new Comment()
                {
                    Content = "I don't like the sound of 'The Great Gatsby'. It doesn't seem like a good play.",
                    dateCommented = DateTime.Now.AddDays(-8),
                    Post = post1,
                    Member = steve,
                    isApproved = true
                };
                //an unapproved comment
                var comment6 = new Comment()
                {
                    Content = "The behind-the-scenes tour sounds boring. I don't think I'll attend.",
                    dateCommented = DateTime.Now.AddDays(-3),
                    Post = post2,
                    Member = steve,
                    isApproved = false
                };
                var comment7 = new Comment()
                {
                    Content = "I didn't like the review of 'Hamlet'. It was too negative and made me not want to see the play.",
                    dateCommented = DateTime.Now,
                    Post = post3,
                    Member = steve,
                    isApproved = true
                };

                if (!context.Comments.Any())
                {
                    //add each comment to the Comments table
                    context.Comments.Add(comment1);
                    context.Comments.Add(comment2);
                    context.Comments.Add(comment3);
                    context.Comments.Add(comment4);
                    context.Comments.Add(comment5);
                    context.Comments.Add(comment6);
                    context.Comments.Add(comment7);
                }
                //save the changes to the database
                context.SaveChanges();
            }


        }

    }//end of class

}//end of namespace