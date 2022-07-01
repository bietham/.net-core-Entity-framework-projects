using System;

namespace ForumApp.ViewModels
{
    public class AttachmentsCreateViewModel
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime Created { get; set; }

    }
}
