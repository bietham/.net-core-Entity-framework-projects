using System.Collections.Generic;

namespace ForumApp.ViewModels
{
    public class TopicEditViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ForumSectionId { get; set; }
        public List<MessageViewModel> Messages { get; set; }
        public List<int> AddedMessages { get; set; }
    }
}
