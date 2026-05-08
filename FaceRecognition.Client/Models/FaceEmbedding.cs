using System;

namespace FaceRecognition.Client.Models
{
    public class FaceEmbedding
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string PersonId { get; set; } = string.Empty;
        public string ImageId { get; set; } = string.Empty;
        public float[] Embedding { get; set; } = Array.Empty<float>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
