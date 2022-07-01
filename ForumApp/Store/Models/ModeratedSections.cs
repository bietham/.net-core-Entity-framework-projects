using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace ForumApp.Store.Models
{
    public class ModeratedSections
    {
        [Key]
        public int Id { get; set; }
        public int ForumSectionId { get; set; }
        public ForumSections ForumSection { get; set; }
        public string ModeratorName { get; set; }  
        public IdentityUser Moderator { get; set; }



    }
}
