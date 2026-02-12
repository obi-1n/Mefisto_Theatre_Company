using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ONnegha_Mefisto_Theatre_Company.Models
{
    public class Staff : User
    {
        public bool promoted { get; set; }
        public bool isPromoted { get; set; }

        // Navigation property to the posts created by the staff member
        public List<Post> Posts { get; set; }
    }
}