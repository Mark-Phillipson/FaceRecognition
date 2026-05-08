using System;

namespace FaceRecognition.Client.Models
{
    public class MatchResult
    {
        public string EmbeddingId { get; set; } = string.Empty;
        public string ImageId { get; set; } = string.Empty;
        public string PersonId { get; set; } = string.Empty;
        public string PersonName { get; set; } = string.Empty;
        public double Distance { get; set; }
        public string ImageDataUrl { get; set; } = string.Empty;
    }
}
