using System;
using System.ComponentModel.DataAnnotations;

namespace ForumApp.Store.Models
{
    public class Attachments
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FileName { get; set; }
        [Required]
        public string FilePath { get; set; }
        [Required]
        public DateTime Created { get; set; }

        public int MessageId { get; set; }
        public Messages Message { get; set; }



    }
}
