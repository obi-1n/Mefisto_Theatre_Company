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
    public class ModeratorController : Controller
    {
        //instance of the database.
        TheatreDbContext db = new TheatreDbContext();

        //the index action is called when registered moderator clicks the link "Moderator Dashboard"
        //this method is returning a list of comments that where created by all members of the website (using user id)
        [Authorize(Roles = "Moderator")]//only moderators can access this method
        public ActionResult Index()
        {
            //get all members from the database
            var members = db.Users.OfType<Member>().ToList();

            //send the list of comments to the Index view
            return View(members.ToList());
        }

        //View Members Comments
        [Authorize(Roles = "Moderator")]
        public ActionResult ViewComments(string id)
        {
            if (id == null)
            {
                return RedirectToAction("ViewComments", new { id = id });
            }
            //select all comments from the Comments table where the UserId is the same as the id parameter
            var comments = db.Comments
                .Where(c => c.MemberId == id)
                .Include(p => p.Post)
                .Include(p => p.Member);
            if (!comments.Any())
            {
                return HttpNotFound();
            }
            //send the list of comments to the ViewMembersComments view
            return View(comments.ToList());
        }

        //delete comment
        [Authorize(Roles = "Moderator")]
        public ActionResult DeleteComment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            db.Comments.Remove(comment);
            db.SaveChanges();
            return ViewComments(comment.MemberId);
        }

        //Suspend Member action is called when moderator clicks the "Suspend" link next to a comment in the Index view
        //or when moderator clicks the "Suspend" link next to the user in the details view of a comment
        [HttpGet]
        [Authorize(Roles = "Moderator")]
        public async Task<ActionResult> SuspendMember(string id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var userManager = new UserManager<Member>(new UserStore<Member>(db)))
            {
                //get user by id
                Member member = await userManager.FindByIdAsync(id);

                if (member == null)
                {
                    return HttpNotFound();
                }

                //get user's current roles
                var userRoles = await userManager.GetRolesAsync(id);
                string oldRole = userRoles.FirstOrDefault() ?? ""; //only one role per user

                //get all roles from the database
                var items = db.Roles.Where(r => r.Name == "Suspended" || r.Name == "Member").Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Name,
                    Selected = r.Name == oldRole //preselect the user's current role
                }).ToList();

                //build the change role view model object including the list of roles
                //and send it to the view displaying the roles in a dropdownlist with the user's current role preselected
                return View(new ChangeRoleViewModel
                {
                    UserName = member.UserName,
                    Roles = items,
                    OldRole = oldRole,
                });
                
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("SuspendMember")]
        public async Task<ActionResult> SuspendMemberConfirmed(string id, [Bind(Include = "Role")] ChangeRoleViewModel model)
        {

            if (ModelState.IsValid)
            {
                var userManager = new UserManager<Member>(new UserStore<Member>(db));

                Member member = await userManager.FindByIdAsync(id); //get user by id

                //get user's current role
                string oldRole = (await userManager.GetRolesAsync(id)).Single(); // Only ever a single role.

                //if current role is the same with selected role then there is no point to update the database
                if (oldRole == model.Role)
                {
                    return RedirectToAction("Index", "Moderator");
                }
                // Remove user from the old role first.
                await userManager.RemoveFromRoleAsync(id, oldRole);
                //now we are adding the user to the new role
                await userManager.AddToRoleAsync(id, model.Role);

                // Update isSuspended property based on the new role
                if (model.Role == "Suspended")
                {
                    member.isSuspended = true;
                }
                else
                {
                    member.isSuspended = false;
                }
                
                await userManager.UpdateAsync(member);

                return RedirectToAction("Index", "Moderator");
            }
            return View(model);
        }
    }
}