using ONnegha_Mefisto_Theatre_Company.Models;
using System.Collections.Generic;

namespace ONnegha_Mefisto_Theatre_Company.ViewModels
{
    public class UserListViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public System.DateTime RegisteredAt { get; set; }
        public string Role { get; set; }
        public string UserType { get; set; } // "Member" or "Staff"
    }
}
