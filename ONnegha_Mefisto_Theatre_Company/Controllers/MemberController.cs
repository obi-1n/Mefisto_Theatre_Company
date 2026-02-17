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
        [Authorize(Roles = "Member")]
        public ActionResult EditComment(int? id)
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

            // Ensure the member can only edit their own comments
            string currentUserId = User.Identity.GetUserId();
            if (comment.MemberId != currentUserId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            //return the EditComment view that allows editing of staff member details
            return View(new EditCommentViewModel
            {
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
            if (ModelState.IsValid)
            {
                var db = new TheatreDbContext();
                Comment comment = db.Comments.Find(id);

                UpdateModel(comment);//update the comment details by using the model

                comment.dateEdited = DateTime.Now;
                db.Entry(comment).State = EntityState.Modified;
                int result = db.SaveChanges();//update the comment details in the database

                if (result > 0)
                {
                    return RedirectToAction("PostDetails", "Home", new { id = comment.PostId });
                }

            }

            return View(model);
        }


    }
}