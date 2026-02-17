using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ONnegha_Mefisto_Theatre_Company.ViewModels
{
    public class ChangeRoleViewModel
    {
        public string UserName { get; set; }
        public string OldRole { get; set; }
        public ICollection<SelectListItem> Roles { get; set; }

        [Required, Display(Name = "Role")]
        public string Role { get; set; }
    }
}