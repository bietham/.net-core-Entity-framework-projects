using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace ForumApp.Store.Models
{
    public class Messages
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Text { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }

        public IdentityUser Creator { get; set; }
        public IdentityUser Moderator { get; set; }
        public DateTime Modified { get; set; }
        public IEnumerable<Attachments> Attachments { get; set; }
        public int TopicId { get; set; }
        public Topics Topic { get; set; }


    }
}
