using ONnegha_Mefisto_Theatre_Company.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ONnegha_Mefisto_Theatre_Company.Models
{
    public class Post
    {
        [Key]
        public string PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime DatePosted { get; set; }
        public bool approved { get; set; }
        public DateTime LastEdited { get; set; }

        // Navigation property to the Staff who created the post
        [ForeignKey("Staff")]
        public string StaffId { get; set; }
        public Staff Staff { get; set; }

        // Navigation property to the Category associated with the post
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        // Navigation property to the comments associated with the post
        public virtual List<Comment> Comments { get; set; }

    }
}