using Microsoft.AspNet.Identity;
using ONnegha_Mefisto_Theatre_Company.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ONnegha_Mefisto_Theatre_Company.Controllers
{
    public class CommentController : Controller
    {
        private readonly TheatreDbContext db = new TheatreDbContext();

        //---------------------------------------------------------------------------------
        //Create comment
        [Authorize(Roles ="Member")]//only members can create comments
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateComment(int postId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return RedirectToAction("PostDetails", "Home", new { id = postId });

            var userId = User.Identity.GetUserId(); // string

            // Must be a Member to comment
            var member = db.Users.OfType<Member>().SingleOrDefault(m => m.Id == userId);
            if (member == null)
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            var comment = new Comment
            {
                Content = content.Trim(),
                dateCommented = DateTime.Now,
                isApproved = true,
                PostId = postId,

                // Set the relationship so Member is never null later
                Member = member
                // If your Comment model has MemberId too, you can set it as well:
                // MemberId = member.Id
            };

            db.Comments.Add(comment);
            db.SaveChanges();

            return RedirectToAction("PostDetails", "Home", new { id = postId });
        }

        [Authorize(Roles = "Admin,Moderator,Member")]//this only alliws admins moderators and the member that wrote the comment to see it
        public ActionResult Delete(int? id)
        {
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            
            int postId = comment.PostId;
            
            db.Comments.Remove(comment);
            db.SaveChanges();
            return RedirectToAction("PostDetails", "Home", new { id = postId });
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
