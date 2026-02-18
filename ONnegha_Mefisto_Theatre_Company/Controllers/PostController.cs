using ONnegha_Mefisto_Theatre_Company.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ONnegha_Mefisto_Theatre_Company.Controllers
{
    public class PostController : Controller
    {
        private readonly TheatreDbContext db = new TheatreDbContext();
        //---------------------------------------------------------------------------------------
        //delete posts

        [Authorize(Roles = "Admin,Editor")]//this allows only admins and editors to delete posts
        public ActionResult DeletePost(int? id)
        {
            //if the id is null, return a bad request error
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //find the post with the given id
            Post post = db.Posts.Find(id);

            //if the post is not found, return a not found error
            if (post == null)
            {
                return HttpNotFound();
            }

            //if the post is found, return the delete view with the post details
            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("DeletePost")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePostConfirmed(int id)
        {
            //find the post with the given id
            Post post = db.Posts.Find(id);

            //remove the post from the database and save changes
            db.Posts.Remove(post);
            db.SaveChanges();

            //after deleting the post, redirect to the home page
            return RedirectToAction("Index", "Home");
        }
    }
}