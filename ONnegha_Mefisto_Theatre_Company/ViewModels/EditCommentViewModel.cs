using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ONnegha_Mefisto_Theatre_Company.ViewModels
{
    public class EditCommentViewModel
    {
        [Required]
        public string Content { get; set; }

        [Display(Name = "Last Edited")]
        public DateTime dateEdited { get; set; }
    }
}