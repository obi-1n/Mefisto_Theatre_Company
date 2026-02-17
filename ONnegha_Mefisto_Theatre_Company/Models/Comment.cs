using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ONnegha_Mefisto_Theatre_Company.Models
{
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CommentId { get; set; }
        public string Content { get; set; }
        [Display(Name ="Approved")]
        public bool isApproved { get; set; }

        [Display(Name = "Date Commented")]
        public DateTime dateCommented { get; set; }
        [Display(Name = "Last Edited")]
        public DateTime? dateEdited { get; set; }

        //navigation properties
        //each comment is associated with a post
        [ForeignKey("Post")]
        public int PostId { get; set; }
        public Post Post { get; set; }

        //each comment is associated with a member
        //only members can comment, so we need to link the comment to the member who made it
        [ForeignKey("Member")]
        public string MemberId { get; set; }
        public Member Member { get; set; }

    }
}