using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using ONnegha_Mefisto_Theatre_Company.Models;
using ONnegha_Mefisto_Theatre_Company.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Web;
using System.Web.Mvc;

namespace ONnegha_Mefisto_Theatre_Company.Controllers
{
    public class HomeController : Controller
    {
        //create instance of DbContext to acces database
        private TheatreDbContext db = new TheatreDbContext();

        //welcome page
        public ActionResult Welcome()
        {
            return View();
        }

        //home page that displays all the posts from the database, ordered from the most current to old posts
        public ActionResult Index()
        {
            //get all posts, include the categoryfor each post, include the user who created the post
            //and order the posts from the most current to old posts
            var posts = db.Posts.Include(p => p.Category).Include(p => p.Staff).OrderByDescending(p => p.DatePosted);

            //send the list of categories over the index page
            //so we can display them
            ViewBag.Categories = db.Categories.ToList();

            //send the posts collection to the view named Index
            return View(posts.ToList());
        }

        //view post details
        public ActionResult PostDetails(int id)
        {
            //get the user id of the currently logged in user
            var userId = User.Identity.GetUserId();

            //if user is not logged in, send the user to the login page
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            //get the posts details
            var post = db.Posts
                    .Include(p => p.Staff)
                    .Include(p => p.Category)
                    .Include(p => p.Comments)
                    .Include(p => p.Comments.Select(c => c.Member))
                    .SingleOrDefault(p => p.PostId == id);
            //if post member is not found, return not found error
            if (post == null)
            {
                return HttpNotFound();
            }
            //return the view that displays the details of the staff post
            return View(post);
        }

        //become a member of the theatre company, by upgrading from a user to a member role
        [HttpGet]
        public async Task<ActionResult> becomeMember()
        {
            // Get the current user's ID
            var id = User.Identity.GetUserId();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Initialize the user manager to interact with user accounts
            var userManager = new UserManager<User>(new UserStore<User>(db));

            // Find the user by their ID
            User user = await userManager.FindByIdAsync(id);

            // Get the user's current role (assumes only one role per user)
            string oldRole = (await userManager.GetRolesAsync(id)).Single(); //only one role per user

            // Create a list of available roles (only "Member" in this case)
            var items = db.Roles.Where(r => r.Name == "Member").Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Name,
                Selected = r.Name == oldRole
            }).ToList();

            // Return the view with the role change model
            return View(new ChangeRoleViewModel
            {
                UserName = user.UserName,
                Roles = items,
                OldRole = oldRole,
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> becomeMember([Bind(Include = "Role")] ChangeRoleViewModel model)
        {
            // Get the current user's ID
            var id = User.Identity.GetUserId();

            if (ModelState.IsValid)
            {

                // Use a fresh context for user manager
                var userManager = new UserManager<User>(new UserStore<User>(new TheatreDbContext()));
                User user = await userManager.FindByIdAsync(id);
                string oldRole = (await userManager.GetRolesAsync(id)).Single();


                // If the role hasn't changed, redirect to home
                if (oldRole == model.Role)
                {
                    return RedirectToAction("Index", "Home");
                }

                // Create a new Member with the same details as the user
                Member member = new Member
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Street = user.Street,
                    City = user.City,
                    PostCode = user.PostCode,
                    PhoneNumber = user.PhoneNumber,
                    RegisteredAt = user.RegisteredAt,
                    EmailConfirmed = user.EmailConfirmed,
                    PasswordHash = user.PasswordHash,
                    SecurityStamp = user.SecurityStamp,
                    dateRegisteredAsMember = DateTime.Now,
                    isSuspended = false
                };

                // Assign the Member role to the newly created member
                await userManager.RemoveFromRoleAsync(id, oldRole);
                await userManager.AddToRoleAsync(id, "Member");

                if (model.Role == "Member")
                {

                    //this was the only way i could fix this issue

                    // Use a separate context so EF isn't tracking the old entity type
                    // Update the database to change the discriminator and member-specific fields
                    using (var ctx = new TheatreDbContext())
                    {
                        ctx.Database.ExecuteSqlCommand(
                            @"UPDATE AspNetUsers
			                SET Discriminator = @disc,
				            dateRegisteredAsMember = @dt,
				            isSuspended = @susp
			                WHERE Id = @id",
                            new SqlParameter("@disc", "Member"),
                            new SqlParameter("@dt", DateTime.Now),
                            new SqlParameter("@susp", false),
                            new SqlParameter("@id", id)
                        );
                    }

                    // Sign out the user so they can sign back in with new role
                    HttpContext.GetOwinContext()
                        .Authentication
                        .SignOut(DefaultAuthenticationTypes.ApplicationCookie);

                    return RedirectToAction("Login", "Account");
                }

                return RedirectToAction("Index", "Home");
            }

            // If model validation fails, return the view with errors
            return View(model);
        }

        //edit member details
        [Authorize(Roles = "Member")]
        [HttpGet]
        public ActionResult EditDetails()
        {
            // Get the current member's ID
            var id = User.Identity.GetUserId();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //find Member in the database by the id
            Member member = db.Users.Find(id) as Member;

            if (member == null)
            {
                return HttpNotFound();
            }

            //send customer details to the view
            return View(new EditDetailsViewModel
            {
                FirstName = member.FirstName,
                LastName = member.LastName,
                Street = member.Street,
                PostCode = member.PostCode,
                City = member.City,
                PhoneNumber = member.PhoneNumber
            });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditDetails([Bind(Include = "FirstName, LastName, Street, City, PostCode, PhoneNumber")] EditDetailsViewModel model)
        {
            //instance of user manager to manage users
            ApplicationUserManager UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

            // Get the current member's ID
            var id = User.Identity.GetUserId();

            if (ModelState.IsValid)
            {
                //find member by id
                Member member = (Member)await UserManager.FindByIdAsync(id);//find user by id and cast it as a customer

                UpdateModel(member);//update the new member details by using the model

                IdentityResult result = await UserManager.UpdateAsync(member);//update the new member details the database
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            // If model validation fails, return the view with errors
            return View(model);
        }

        //about page
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        //contact page
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}