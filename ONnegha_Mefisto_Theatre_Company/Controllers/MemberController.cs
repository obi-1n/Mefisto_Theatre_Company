using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
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
    public class MemberController : Controller
    {
        private TheatreDbContext db = new TheatreDbContext();

        // GET: Member
        public ActionResult Index()
        {
            return View();
        }

        // GET: Member/EditComment/5
        //edit comment details
        [Authorize(Roles = "Member")]
        public ActionResult EditComment(int? id)
        {
            //check if the id is null, if it is return a bad request error
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //find the comment in the database using the id
            Comment comment = db.Comments.Find(id);

            //check if the comment is null, if it is return a not found error
            if (comment == null)
            {
                return HttpNotFound();
            }

            // Ensure the member can only edit their own comments
            string currentUserId = User.Identity.GetUserId();
            if (comment.MemberId != currentUserId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            //return the EditComment view that allows editing of staff member details
            return View(new EditCommentViewModel
            {
                //populate the EditCommentViewModel with the comment details
                Content = comment.Content,
                dateEdited = DateTime.Now
            });
        }

        // POST: Member/EditComment/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public ActionResult EditComment(int id,
            [Bind(Include = "Content")] EditCommentViewModel model)
        {
            //check if the model state is valid
            if (ModelState.IsValid)
            {
                //find the comment in the database using the id
                var db = new TheatreDbContext();
                Comment comment = db.Comments.Find(id);

                //update the comment details by using the model
                UpdateModel(comment);

                //set the date edited to the current date and time
                comment.dateEdited = DateTime.Now;

                //mark the comment as modified
                db.Entry(comment).State = EntityState.Modified;

                //update the comment details in the database
                int result = db.SaveChanges();

                //if the update is successful, redirect to the post details page of the post that the comment belongs to
                if (result > 0)
                {
                    return RedirectToAction("PostDetails", "Home", new { id = comment.PostId });
                }

            }

            //if the model state is not valid, return the EditComment view with the model to display validation errors
            return View(model);
        }


    }
}