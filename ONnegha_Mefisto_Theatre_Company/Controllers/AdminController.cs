using ONnegha_Mefisto_Theatre_Company.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

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



        // GET: Posts/Delete/5
        public ActionResult DeletePost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("DeletePost")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePostConfirmed(int id)
        {
            Post post = db.Posts.Find(id);
            db.Posts.Remove(post);
            db.SaveChanges();
            return RedirectToAction("ViewAllPosts");
        }
        // GET: Admin/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Admin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Admin/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Admin/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
