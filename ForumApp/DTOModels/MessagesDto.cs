using System;
using System.Text.Json.Serialization;

namespace ForumApp.DTOModels
{
    public class MessagesDto
    {

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("created")]
        public DateTime Created { get; set; }

        [JsonPropertyName("modified")]
        public DateTime Modified { get; set; }
    }
}
