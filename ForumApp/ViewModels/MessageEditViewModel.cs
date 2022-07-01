using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ForumApp.ViewModels
{
    public class MessageEditViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; }

        public DateTime Modified { get; set; }

        [Display(Name = "Add Attachments")]
        public List<IFormFile> Attachments { get; set; }
        public List<int> AddedAttachments { get; set; }
        public int TopicId { get; set; }

    }
}
