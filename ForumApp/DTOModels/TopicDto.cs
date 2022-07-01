using System;
using System.Text.Json.Serialization;

namespace ForumApp.DTOModels
{
    public class TopicDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("created")]
        public DateTime Created { get; set; }

    }
}
