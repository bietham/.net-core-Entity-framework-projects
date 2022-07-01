using System.Text.Json.Serialization;

namespace ForumApp.DTOModels
{
    public class ForumSectionDto
    {

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

    }

}
