using System.Threading.Tasks;
using FaceRecognition.Client.Models;

namespace FaceRecognition.Client.Services
{
    public interface IFaceService
    {
        Task EnsureLoadedAsync();
        Task LoadModelsAsync(string modelBaseUrl);
        Task<FaceDetectionResult?> DetectSingleFaceEmbeddingAsync(string dataUrl);
        Task<List<FaceDetectionResult>> DetectMultipleFacesEmbeddingAsync(string dataUrl);
        Task<List<FaceDetectionResult>> DetectMultipleFacesEmbeddingAsync(string dataUrl, int thumbnailMaxSize);
    }
}
