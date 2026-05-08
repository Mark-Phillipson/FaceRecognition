using System;

namespace FaceRecognition.Client.Models
{
    public class FaceImage
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string PersonId { get; set; } = string.Empty;
        public string DataUri { get; set; } = string.Empty; // data:image/png;base64,...
        public int Width { get; set; }
        public int Height { get; set; }
        // optional metadata for group photos
        public string SourceImageId { get; set; } = string.Empty;
        public int SourceX { get; set; }
        public int SourceY { get; set; }
        public int SourceWidth { get; set; }
        public int SourceHeight { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
