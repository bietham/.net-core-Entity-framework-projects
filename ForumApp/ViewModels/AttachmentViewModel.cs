using System;

namespace ForumApp.ViewModels
{
    public class AttachmentViewModel
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime Created { get; set; }
        public int MessageId { get; set; }
        public MessageViewModel Message { get; set; }
    }
}
