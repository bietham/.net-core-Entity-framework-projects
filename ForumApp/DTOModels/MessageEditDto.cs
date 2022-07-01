using System.Text.Json.Serialization;

namespace ForumApp.DTOModels
{
    public class MessageEditDto
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}
