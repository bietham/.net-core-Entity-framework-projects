using System.ComponentModel.DataAnnotations;

namespace ForumApp.ViewModels
{
    public class ForumSectionCreateViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="Name of a forum section can not be empty")]
        public string Name { get; set; }

        [Required(ErrorMessage ="Enter a description for the forum section")]
        public string Description { get; set; }


    }
}
