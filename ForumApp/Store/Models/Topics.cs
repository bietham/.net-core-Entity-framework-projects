using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace ForumApp.Store.Models
{
    public class Topics
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public IdentityUser Creator { get; set; }
        public IdentityUser Moderator { get; set; }
        public IEnumerable<Messages>Messages {get;set;}
        public int ForumSectionId { get; set; }
        public ForumSections ForumSection  { get; set; }

    }
}
