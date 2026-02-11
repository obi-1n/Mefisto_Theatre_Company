using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ONnegha_Mefisto_Theatre_Company.Models.ViewModels
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Display(Name = "Category")]
        public string Name { get; set; }

        // Navigation property to the posts associated with the category
        public virtual List<Post> Posts { get; set; }
    }
}