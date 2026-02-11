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

                ////if suspended role doesnt exist, create it
                ////suspended users can no longer leave comments, they can still view content but they cannot interact with content in any way
                //if (!roleManager.RoleExists("Suspended"))
                //{
                //    roleManager.Create(new IdentityRole("Suspended"));
                //}




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
                if (userManager.FindByName("editor1@MTC.com") == null)
                {
                    var jeff = new Staff
                    {
                        UserName = "editor1@MTC.com",
                        Email = "editor1@MTC.com",
                        FirstName = "Jeff",
                        LastName = "Jefferson",
                        Street = "156 Gladys St",
                        City = "Glasgow",
                        PostCode = "G9 7GG",
                        RegisteredAt = new DateTime(2022, 2, 11),
                        EmailConfirmed = true,
                        PhoneNumber = "07355829778",
                        isPromoted = false
                    };

                    //add jeff and the password to the Users table
                    userManager.Create(jeff, "editor123");
                    //assign the editor role to jeff
                    userManager.AddToRole(jeff.Id, "Editor");
                }

                if (userManager.FindByName("editor2@MTC.com") == null)
                {
                    var jane = new Staff
                    {
                        UserName = "editor2@MTC.com",
                        Email = "editor2@MTC.com",
                        FirstName = "Jane",
                        LastName = "Jenkins",
                        Street = "6 Elmo St",
                        City = "Glasgow",
                        PostCode = "G1 7UM",
                        RegisteredAt = new DateTime(2023, 5, 1),
                        EmailConfirmed = true,
                        PhoneNumber = "07355856098",
                        isPromoted = false
                    };

                    //add jane and the password to the Users table
                    userManager.Create(jane, "editor456");
                    //assign the editor role to jane
                    userManager.AddToRole(jane.Id, "Editor");
                }

                //create a moderator user
                //check if the moderator user already exists, if not create it and assign it to the moderator role
                if (userManager.FindByName("moderator@MTC.com") == null)
                {
                    var michael = new Staff
                    {
                        UserName = "moderator@MTC.com",
                        Email = "moderator@MTC.com",
                        FirstName = "Michael",
                        LastName = "Phelps",
                        Street = "34 Olympic St",
                        City = "Glasgow",
                        PostCode = "G2 7GG",
                        RegisteredAt = new DateTime(2021, 8, 15),
                        EmailConfirmed = true,
                        PhoneNumber = "07355812345",
                        isPromoted = false
                    };

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
                    RegisteredAt = new DateTime(2025, 1, 1),
                    dateRegisteredAsMember = new DateTime(2025, 1, 9),
                    EmailConfirmed = true,
                    PhoneNumber = "07563902144"
                };
                if (userManager.FindByName("bill@gmail.com") == null)
                {
                    userManager.Create(bill, "member1");
                    userManager.AddToRole(bill.Id, "Member");
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
                var steve = new User
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
                    PhoneNumber = "07988441227"
                };
                if (userManager.FindByName("steveb@gmail.com") == null)
                {

                    userManager.Create(steve, "user1");
                    userManager.AddToRoles(steve.Id, "User");
                }

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

                context.SaveChanges();

            }

        }

    }//end of class

}//end of namespace