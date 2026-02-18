using Microsoft.AspNet.Identity;
using ONnegha_Mefisto_Theatre_Company.Models;
using System;
using System.Linq;
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
            // Basic validation: ensure content is not empty and postId is valid
            if (string.IsNullOrWhiteSpace(content))
                return RedirectToAction("PostDetails", "Home", new { id = postId });

            // Get the current user's ID
            var memberId = User.Identity.GetUserId();

            // Must be a Member entity to comment (not just "in the Member role")
            var member = db.Users.OfType<Member>().SingleOrDefault(m => m.Id == memberId);
            if (member == null)
            {
                // You can choose Redirect instead; returning 403 makes it clear it's permission-related.
                return new HttpStatusCodeResult(403, "You must be a Member to comment. Please sign out and sign in again, or contact support.");
            }

            // Create the comment and set its properties, including the relationship to the Member
            var comment = new Comment
            {
                Content = content.Trim(),
                dateCommented = DateTime.Now,
                isApproved = true,
                PostId = postId,

                // Set the relationship so Member is never null later
                MemberId = member.Id,
                Member = member
            };

            // Save the comment to the database
            db.Comments.Add(comment);
            db.SaveChanges();

            return RedirectToAction("PostDetails", "Home", new { id = postId });
        }

        [Authorize(Roles = "Admin,Moderator,Member") ]//this only allows admins, moderators and the member that wrote the comment to see it
        public ActionResult Delete(int? id)
        {
            // Validate the comment ID
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }

            // Check if the current user is authorized to delete the comment
            int postId = comment.PostId;

            // Admins and Moderators can delete any comment, but Members can only delete their own comments
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
