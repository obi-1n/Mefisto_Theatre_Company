using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ONnegha_Mefisto_Theatre_Company.Models
{
    public class TheatreDbContext : IdentityDbContext<User>
    {
        public TheatreDbContext()
            : base("TheatreConnection", throwIfV1Schema: false)
        {
        }
        public static TheatreDbContext Create()
        {
            return new TheatreDbContext();
        }
    }
}