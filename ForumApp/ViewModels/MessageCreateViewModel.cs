using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace ForumApp.ViewModels
{
    public class MessageCreateViewModel
    {

        [Required(ErrorMessage = "Your message can not be empty")]
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<IFormFile> Attachments { get; set; }
        public int TopicId { get; set; }
    }
}
