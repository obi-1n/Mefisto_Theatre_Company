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
using System.Web;
using System.Web.Mvc;

namespace ONnegha_Mefisto_Theatre_Company.Controllers
{
    public class HomeController : Controller
    {
        //create instance of DbContext to acces database
        private TheatreDbContext db = new TheatreDbContext();

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
        
        [HttpGet]
        public async Task<ActionResult> becomeMember()
        {
            var id = User.Identity.GetUserId();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userManager = new UserManager<User>(new UserStore<User>(db));

            User user = await userManager.FindByIdAsync(id);

            string oldRole = (await userManager.GetRolesAsync(id)).Single(); //only one role per user

            var items = db.Roles.Where(r => r.Name == "Member").Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Name,
                Selected = r.Name == oldRole
            }).ToList();

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
            var id = User.Identity.GetUserId();

            if (ModelState.IsValid)
            {
                var userManager = new UserManager<User>(new UserStore<User>(db));

                string oldRole = (await userManager.GetRolesAsync(id)).Single();

                if (oldRole == model.Role)
                {
                    return RedirectToAction("Index", "Home");
                }

                await userManager.RemoveFromRoleAsync(id, oldRole);
                await userManager.AddToRoleAsync(id, model.Role);

                if (model.Role == "Member")
                {
                    HttpContext.GetOwinContext()
                        .Authentication
                        .SignOut(DefaultAuthenticationTypes.ApplicationCookie);

                    return RedirectToAction("Login", "Account");
                }

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        [Authorize(Roles = "Member")]
        [HttpGet]
        public ActionResult EditDetails()
        {
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
            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}