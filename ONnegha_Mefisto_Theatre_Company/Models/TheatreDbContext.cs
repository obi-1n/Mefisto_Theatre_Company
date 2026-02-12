using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ONnegha_Mefisto_Theatre_Company.Models
{
    public class TheatreDbContext : IdentityDbContext<User>
    {
        //define DbSet properties for each entity in the application
        public DbSet<Member> Members { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Category> Categories { get; set; }

        public TheatreDbContext()
            : base("TheatreConnection", throwIfV1Schema: false)
        {
            Database.SetInitializer(new DatabaseInitialiser());
        }
        public static TheatreDbContext Create()
        {
            return new TheatreDbContext();
        }
    }
}