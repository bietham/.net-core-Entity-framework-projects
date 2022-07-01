using System.Text.Json.Serialization;

namespace ForumApp.DTOModels
{
    public class TopicCreateDto
    {

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}
