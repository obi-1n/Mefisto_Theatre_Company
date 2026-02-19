using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
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
    [Authorize(Roles = "Admin")]//this allows only registered users as admin role to access the Admin controller
    public class AdminController : Controller
    {
        //create an instance of the database so we can access the tables
        private TheatreDbContext db = new TheatreDbContext();

        // GET: Admin
        [Authorize(Roles = "Admin")]//only Admins can call the index action,
        public ActionResult Index()
        {
            return View();
        }

        //******************************************************************************************************

        //STAFF
        // GET: Staff
        //view all staff members
        [Authorize(Roles = "Admin")]//only Admins can call the ViewAllStaff action,
        public ActionResult ViewAllStaff()
        {
            //get all staff members and order them by their first name
            var staff = db.Staffs.OrderBy(s => s.FirstName).ToList();

            //return the ViewAllStaff view that displays a list of staff members
            return View(staff);
        }


        //create a new staff member
        [Authorize(Roles = "Admin")]//only Admins can call the CreateStaff action,
        [HttpGet]
        public ActionResult CreateStaff()
        {
            //create a new instance of the CreateStaffViewModel
            var model = new CreateStaffViewModel
            {
                //populate the Roles property with only admin,moderator, and editor roles from the database
                Roles = db.Roles.Where(r => r.Name == "Admin" || r.Name == "Moderator" || r.Name == "Editor")
                                .Select(r => new SelectListItem
                                {
                                    Value = r.Name,
                                    Text = r.Name
                                }).ToList()
            };
            //return the CreateStaff view and pass the model to it
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateStaff(CreateStaffViewModel model)
        {
            //if model is not null
            if (ModelState.IsValid)
            {
                //build employee object from the view model
                var staff = new Staff
                {
                    UserName = model.Email,
                    Email = model.Email,
                    EmailConfirmed = true,
                    Street = model.Street,
                    City = model.City,
                    PostCode = model.PostCode,
                    PhoneNumber = model.PhoneNumber,
                    PhoneNumberConfirmed = true,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    RegisteredAt = DateTime.Now
                };
                
                //create a local UserManager instance that will be properly disposed
                var userManager = new UserManager<Staff>(new UserStore<Staff>(db));
                
                //create the user using the UserManager service
                var result = userManager.Create(staff, model.Password);
                //if user creation succeeded
                if (result.Succeeded)
                {
                    //assign the selected role to the user
                    userManager.AddToRole(staff.Id, model.Role);
                    //redirect to the admin index page after successful creation
                    return RedirectToAction("Index");
                }
                else
                {
                    //add error messages from UserManager to ModelState
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
            }
            // Always repopulate Roles before returning the view
            model.Roles = db.Roles.Where(r => r.Name == "Admin" || r.Name == "Moderator" || r.Name == "Editor")
                                    .Select(r => new SelectListItem
                                    {
                                        Value = r.Name,
                                        Text = r.Name
                                    }).ToList();
            //if we got this far, something failed, redisplay form
            return View(model);
        }


        //view staff member details
        public ActionResult StaffDetails(string id)
        {
            //if id is null, return bad request error
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //find the staff member by id
            Staff staff = db.Staffs.Find(id);
            //if staff member is not found, return not found error
            if (staff == null)
            {
                return HttpNotFound();
            }
            //return the ViewStaffDetails view that displays the details of the staff member
            return View(staff);
        }   


        //edit staff member details
        [Authorize (Roles = "Admin")]//only Admins can call the EditStaff action,
        [HttpGet]
        public ActionResult EditStaff(string id)
        {
            //if id is null, return bad request error
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //find the staff member by id
            Staff staff = db.Staffs.Find(id);

            //if staff member is not found, return not found error
            if (staff == null)
            {
                return HttpNotFound();
            }
            //return the EditStaff view that allows editing of staff member details
            return View(new EditStaffViewModel
            {
                FirstName = staff.FirstName,
                LastName = staff.LastName,
                Street = staff.Street,
                City = staff.City,
                PostCode = staff.PostCode,
                PhoneNumber = staff.PhoneNumber
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditStaff(string id,
            [Bind(Include = "FirstName, LastName, Street, City, PostCode, PhoneNumber")] EditStaffViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userManager = new UserManager<Staff>(new UserStore<Staff>(db));
                Staff staff = await userManager.FindByIdAsync(id);

                UpdateModel(staff);//update the new staff details by using the model

                IdentityResult result = await userManager.UpdateAsync(staff);//update the new staff details the database

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Admin");
                }

            }
            return View(model);
        }

        
        //delete staff member

        [Authorize(Roles = "Admin")]//only Admins can call the DeleteStaff action,
        public async Task<ActionResult> DeleteStaff(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //check we aren't deleting our own account
            if (id == User.Identity.GetUserId())
            {
                return RedirectToAction("Index", "Admin");
            }

            //create a local UserManager instance that will be properly disposed
            var userManager = new UserManager<Staff>(new UserStore<Staff>(db));

            //find user by id in the database
            Staff staff = await userManager.FindByIdAsync(id);//get user by id
            if (staff == null)
            {
                return HttpNotFound();
            }

            //delete all posts created by the staff member first to avoid constraint violation
            var posts = db.Posts.Where(p => p.StaffId == id).ToList();
            foreach (var post in posts)
            {
                //delete comments for each post
                var comments = db.Comments.Where(c => c.PostId == post.PostId).ToList();
                db.Comments.RemoveRange(comments);
            }
            db.Posts.RemoveRange(posts);
            db.SaveChanges();

            //delete the user
            await userManager.DeleteAsync(staff);

            return RedirectToAction("Index", "Admin");
        }

        
        //change staff member role
        [Authorize(Roles = "Admin")]//only Admins can call the ChangeRole action,
        [HttpGet]
        public async Task<ActionResult> ChangeRole(string id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //cant change own role
            if (id == User.Identity.GetUserId())
            {
                return RedirectToAction("Index", "Admin");
            }

            var userManager = new UserManager<Staff>(new UserStore<Staff>(db));

            //get user by id
            User user = await userManager.FindByIdAsync(id);

            //get user's current roles
            string oldRole = (await userManager.GetRolesAsync(id)).Single(); //only one role per user

            //get all roles from the database
            var items = db.Roles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Name,
                Selected = r.Name == oldRole //preselect the user's current role
            }).ToList();

            //build the change role view model object including the list of roles
            //and send it to the view displaying the roles in a dropdownlist with the user's current role preselected
            return View(new ChangeRoleViewModel
            {
                UserName = user.UserName,
                Roles = items,
                OldRole = oldRole,
            });
        }

        //POST: Admin/ChangeRole/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeRole(string id, [Bind(Include = "Role")] ChangeRoleViewModel model)
        {
            // Can't change your own role.
            if (id == User.Identity.GetUserId())
            {
                return RedirectToAction("Index", "Admin");
            }

            if (ModelState.IsValid)
            {
                var userManager = new UserManager<Staff>(new UserStore<Staff>(db));

                Staff staff = await userManager.FindByIdAsync(id); //get user by id

                //get user's current role
                string oldRole = (await userManager.GetRolesAsync(id)).Single(); // Only ever a single role.

                //if current role is the same with selected role then there is no point to update the database
                if (oldRole == model.Role)
                {
                    return RedirectToAction("Index", "Admin");
                }
                // Remove user from the old role first.
                await userManager.RemoveFromRoleAsync(id, oldRole);
                //now we are adding the user to the new role
                await userManager.AddToRoleAsync(id, model.Role);

                // If the new role is Admin, set isPromoted to true
                if (model.Role == "Admin")
                {
                    staff.isPromoted = true;
                    await userManager.UpdateAsync(staff);
                }
                else
                {
                    staff.isPromoted = false;
                    await userManager.UpdateAsync(staff);
                }

                    return RedirectToAction("Index", "Admin");
            }
            return View(model);
        }

        //******************************************************************************************************
        //Users
        // GET: Users

        public ActionResult ViewAllUsers()
        {
            // Get all users with their roles
            var users = db.Users.OrderBy(u => u.FirstName).ToList();
            var userList = new List<UserListViewModel>();

            foreach (var user in users)
            {
                // Get the user's role
                string role = db.Roles.FirstOrDefault(r => r.Users.Any(u => u.UserId == user.Id))?.Name;

                // Filter to only display users with the role member user or suspended
                if (role == "Member" || role == "User" || role == "Suspended")
                {
                    userList.Add(new UserListViewModel
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        UserName = user.UserName,
                        RegisteredAt = user.RegisteredAt,
                        Role = role,
                        UserType = user is Staff ? "Staff" : (user is Member ? "Member" : "User")
                    });
                }
            }

            // Return the ViewAllUsers view with the list of UserListViewModel
            return View(userList);
        }

        // Delete User
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // check we aren't deleting our own account
            if (id == User.Identity.GetUserId())
            {
                return RedirectToAction("ViewAllUsers");
            }

            //find user by id
            User user = db.Users.Find(id);

            //if user id is null give error message
            if (user == null)
            {
                return HttpNotFound();
            }

            // Use appropriate logic based on user type
            if (user is Member member)
            {
                // Deleting a member: delete their comments first
                var comments = db.Comments.Where(c => c.MemberId == id).ToList();
                db.Comments.RemoveRange(comments);
                db.SaveChanges();

                var userManager = new UserManager<User>(new UserStore<User>(db));
                await userManager.DeleteAsync(member);
            }
            else
            {
                // Just a base User 
                var userManager = new UserManager<User>(new UserStore<User>(db));
                await userManager.DeleteAsync(user);
            }

            return RedirectToAction("ViewAllUsers");
        }

        //******************************************************************************************************
        //Posts

        //view all posts in a category
        [Authorize(Roles = "Admin")] //only Admins can call the ViewAllPosts action,
        public ActionResult ViewAllPosts()
        {
            //get all posts from database including their category and staff who created post
            List<Post> posts = db.Posts.Include(p => p.Category).Include(p => p.Staff).ToList();

            //send the list to the view named ViewAllPosts
            return View(posts);
        }
        
    }
}
