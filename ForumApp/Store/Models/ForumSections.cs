using System.Collections.Generic;

namespace ForumApp.Store.Models
{
    public class ForumSections
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public IEnumerable<Topics> Topics { get; set; }
    }
}
