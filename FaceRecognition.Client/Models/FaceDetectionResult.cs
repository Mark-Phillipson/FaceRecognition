using System;

namespace FaceRecognition.Client.Models
{
    public class FaceDetectionResult
    {
        public float[] Descriptor { get; set; } = Array.Empty<float>();
        public BoundingBox Box { get; set; } = new BoundingBox();
        public string ThumbnailDataUrl { get; set; } = string.Empty;
    }

    public class BoundingBox
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
    }
}
