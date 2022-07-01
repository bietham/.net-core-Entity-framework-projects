using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ForumApp.ViewModels
{
    public class MessageViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime Modified { get; set; }
        public List<AttachmentViewModel> Attachments { get; set; }
        public IdentityUser Creator { get; set; }
        public IdentityUser Moderator { get; set; }
        public int TopicId { get; set; }
        public TopicViewModel Topic { get; set; }

    }
}
