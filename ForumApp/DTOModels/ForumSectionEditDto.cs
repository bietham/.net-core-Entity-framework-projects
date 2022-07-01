using System.Text.Json.Serialization;

namespace ForumApp.DTOModels
{
    public class ForumSectionEditDto
    {

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}
