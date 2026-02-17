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

        //-----------------------------------------------------------------------------

        //create a new staff member
        [Authorize(Roles = "Admin")]
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

        //-----------------------------------------------------------------------------

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

        //-----------------------------------------------------------------------------

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

        //-----------------------------------------------------------------------------
        //delete staff member
        public async Task<ActionResult> DeleteStaff(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //check we arent deleting our own account
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

            //delete the user
            await userManager.DeleteAsync(staff);

            return RedirectToAction("Index", "Admin");
        }

        //-----------------------------------------------------------------------------
        //change staff member role
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

        //Post: Users/ChangeRole/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("ChangeRole")]
        public async Task<ActionResult> ChangeRoleConfirmed(string id, [Bind(Include = "Role")] ChangeRoleViewModel model)
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

                return RedirectToAction("Index", "Admin");
            }
            return View(model);
        }

        //******************************************************************************************************
        //CATEGORIES
        // GET: Categories

        //view all categories
        [Authorize(Roles = "Admin")]//only Admins can call the ViewAllCategories action,
        public ActionResult ViewAllCategories()
        {
            //return the ViewAllCategories view that displays a list of categories
            return View(db.Categories.ToList());
        }

        //view all posts in a category
        [Authorize(Roles = "Admin")] //only Admins can call the ViewAllPosts action,
        public ActionResult ViewAllPosts()
        {
            //get all posts from database including their category and staff who created post
            List<Post> posts = db.Posts.Include(p => p.Category).Include(p => p.Staff).ToList();

            //send the list to the view named ViewAllPosts
            return View(posts);
        }

        //view all comments for a post
        [Authorize(Roles = "Admin")] //only Admins can call the ViewAllComments action,
        public ActionResult ViewAllComments(int? id)
        {
            //get all comments
            List<Comment> comments = db.Comments.Include(c => c.Member).Include(c => c.Post).ToList();
            //send the list to the view named ViewAllComments
            return View(comments);
        }


        //// GET: Admin/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Admin/Create
        //[HttpPost]
        //public ActionResult Create(FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add insert logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: Admin/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        //// POST: Admin/Edit/5
        //[HttpPost]
        //public ActionResult Edit(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add update logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: Admin/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: Admin/Delete/5
        //[HttpPost]
        //public ActionResult Delete(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add delete logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}
