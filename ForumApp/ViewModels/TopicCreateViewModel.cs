using ForumApp.Store.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ForumApp.ViewModels
{
    public class TopicCreateViewModel
    {

        [Required(ErrorMessage = "Please enter the name for your topic")]
        public string Name { get; set; }

        [Required(ErrorMessage ="Please enter the description for your topic")]
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ForumSectionId { get; set; }
        public ForumSections ForumSection { get; set; }
    }
}
