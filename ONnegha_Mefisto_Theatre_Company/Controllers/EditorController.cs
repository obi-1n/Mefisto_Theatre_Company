using Microsoft.AspNet.Identity;
using ONnegha_Mefisto_Theatre_Company.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ONnegha_Mefisto_Theatre_Company.Controllers
{
    public class EditorController : Controller
    {
        //instance of the database.
        private TheatreDbContext db = new TheatreDbContext();

        // GET: Posts
        //the index action is called when registered user click the link "My Posts"
        //this method is returning a list of posts that where created by the logged in user (using user id)
        [Authorize(Roles = "Editor")]//only editors can access this method
        public ActionResult Index()
        {

            //select all the posts from the Posts table
            //including the foreign keys category and User
            var posts = db.Posts
            .Include(p => p.Category)
            .Include(p => p.Staff);

            //get the Id of the logged in user, using Identity
            //user Id is a string
            var staffId = User.Identity.GetUserId();

            //from the list of posts from the Posts tables
            //select only the ones that have the UserId equall to the Id of the logged in user
            //returns a list of posts
            posts = posts.Where(p => p.StaffId == staffId);

            //send the list of posts to the Index view in Editors subfolder
            return View(posts.ToList());
        }

        // GET: Posts/Details/5
        public ActionResult Details(int? id) //? creates a nullable variable
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //find a post in the Posts table by id
            Post post = db.Posts.Find(id);

            //if post doesn't exists then return a not found error message
            if (post == null)
            {
                return HttpNotFound();

            }
            //otherwise send the post to the Details view
            //and display the values stored in the properties
            return View(post);

        }

        // GET: Editor/Create
        public ActionResult Create()
        {
            //send the list of categories to the view using a ViewBag
            //so user can select the category for the post from a dropdown box.
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name");

            //return the Create view to the browser
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PostId, Title, Content, CategoryId")] Post post)
        {
            //if parameter post is not null
            if (ModelState.IsValid)
            {
                //assign todays date for DatePosted
                post.DatePosted = DateTime.Now;

                //assign null for LastEdited because the post is being created for the first time post.LastEdited = null;
                post.LastEdited = null;

                //assign the registered user id as a foreignkey
                //this is the user who creates the post
                post.StaffId = User.Identity.GetUserId();

                //add the post to the Posts tables
                db.Posts.Add(post);

                //save changes in databse
                db.SaveChanges();

                //return to Index action in MemberController
                return RedirectToAction("Index");

            }

            //if the parameter post is null then send the list categories back to the create view
            //and try to create a post again
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name", post.CategoryId);

            //send the post back to the create view.
            return View(post);

        }

        // GET: Posts/Edit/5
        //this method returns the Edit form to the browser
        //togheter with the an instance of post
        //so user can make changes
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //find post by Id in the Posts table
            Post post = db.Posts.Find(id);

            if (post == null)
            {
                return HttpNotFound();
            }

            //get a list of all the categories from Categories table
            //and send the list to the view using a ViewBag
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name", post.CategoryId);

            //also send the post to the Edit View
            //where user can change the details of the post
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //this method gets the edited/modified post and updates the changes in the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PostId, Title, Content, CategoryId")] Post post)
        {
            //if the post passed as a parameter to the Edit action is not null then
            //the edited post will be updated in tha database
            if (ModelState.IsValid)
            {
                var originalPost = db.Posts.Find(post.PostId);
                if (originalPost == null)
                {
                    return HttpNotFound();
                }

                // Update only the properties that should change
                originalPost.Title = post.Title;
                originalPost.Content = post.Content;
                originalPost.CategoryId = post.CategoryId;
                originalPost.LastEdited = DateTime.Now;
                originalPost.StaffId = User.Identity.GetUserId();
                // DatePosted remains unchanged

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //otherwise, if the post parameter is null, then
            //we send the list of categories back to the Edit form and
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name", post.CategoryId);

            //return the post to the Edit form
            return View(post);
        }

        // GET: Posts/Delete/5
        //this method will delete a post by id
        public ActionResult Delete(int? id)
        {
            //if id is null then return a bad request error
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //first find a post in the Posts tables by id
            Post post = db.Posts.Find(id);

            //next find the post category by seraching through the Categories table
            // for a category by id which is the foreign key in that post
            var category = db.Categories.Find(post.CategoryId);

            //assign the category to the Category navigational property Category
            //so we can display the category name
            post.Category = category;

            //if the post is a null object
            //then return a not found error message
            if (post == null)
            {
                return HttpNotFound();
            }
            //otherwise return the Delete view and send the post to the view
            //so post details can be viwed
            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]//this is important
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //find post by id in Posts tables
            Post post = db.Posts.Find(id);

            //remove post from Posts table
            db.Posts.Remove(post);

            //save changes in tha database
            db.SaveChanges();

            //redirect ti Index action in MemberController
            return RedirectToAction("Index");


        }
    }
}
