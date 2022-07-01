using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace ForumApp.ViewModels
{
    public class ForumSectionEditViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IdentityUser Moderator {get; set;}
        public string ModeratorEmail { get; set; }
        public List<TopicViewModel> Topics { get; set; }
        public List<int> AddedTopics { get; set; }
    }
}
