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
        [Authorize(Roles = "Editor")]//only editors can access this method
        public ActionResult Index()
        {

            //select all the posts from the Posts table
            //including the foreign keys category and User
            var posts = db.Posts
            .Include(p => p.Category)
            .Include(p => p.Staff);

            //get the Id of the logged in editor, using Identity
            //user Id is a string
            var staffId = User.Identity.GetUserId();

            //from the list of posts from the Posts tables
            //select only the ones that have the UserId equall to the Id of the logged in user
            //returns a list of posts
            posts = posts.Where(p => p.StaffId == staffId);

            //send the list of posts to the Index view in Editors subfolder
            return View(posts.ToList());
        }

        //---------------------------------------------------------------------------------------------------
        //Post
        // GET: Posts/Details/5
        //post details
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
        //create a new post
        public ActionResult Create()
        {
            //send the list of categories to the view using a ViewBag
            //so user can select the category for the post from a dropdown box.
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name");

            //return the Create view to the browser
            return View();
        }

        // POST: Posts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PostId, Title, Content, CategoryId")] Post post)
        {
            //if the title or content of the post is empty, then the model state will be invalid

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
        //edit post
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

        // -----------------------------------------------------------------------------------------------------------
        //Category
        // Allow Editors to create new categories
        [Authorize(Roles = "Editor")]
        public ActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Editor")]
        public ActionResult CreateCategory([Bind(Include = "Name")] Category category)
        {
            // Validate the model state first
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            // Validate that the category name is not empty or whitespace
            if (string.IsNullOrWhiteSpace(category.Name))
            {
                ModelState.AddModelError("Name", "Category name is required.");
                return View(category);
            }

            // Trim whitespace from the category name
            category.Name = category.Name.Trim();

            // Prevent duplicates (case-insensitive)
            bool exists = db.Categories.Any(c => c.Name.ToLower() == category.Name.ToLower());

            // If a category with the same name already exists, add a model error and return to the view
            if (exists)
            {
                ModelState.AddModelError("Name", "A category with this name already exists.");
                return View(category);
            }

            //add the new category to the database
            db.Categories.Add(category);
            db.SaveChanges();

            // After creating a category, redirect editors back to their dashboard
            return RedirectToAction("Create");
        }

    }
}
