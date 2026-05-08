using System.Collections.Generic;
using System.Threading.Tasks;
using FaceRecognition.Client.Models;

namespace FaceRecognition.Client.Services
{
    public interface IRecognitionService
    {
        Task ComputeEmbeddingsForAllImagesAsync();
        Task<List<MatchResult>> FindMatchesAsync(float[] queryEmbedding, int topN = 5);
    }
}
