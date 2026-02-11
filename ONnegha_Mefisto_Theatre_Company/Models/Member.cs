using ONnegha_Mefisto_Theatre_Company.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ONnegha_Mefisto_Theatre_Company.Models
{
    public class Member : User
    {
        //[Key]
        //public int MemberId { get; set; }

        [Display(Name = "Suspended")]
        public bool IsSuspended { get; set; }

        [Display(Name = "Date Became a member")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime dateRegisteredAsMember { get; set; }

        //navigation properties
        //each member can have many comments
        public virtual List<Comment> Comments { get; set; }
    }
}