using System.Threading.Tasks;
using Microsoft.JSInterop;
using FaceRecognition.Client.Models;

namespace FaceRecognition.Client.Services
{
    public class FaceInteropService : IFaceService
    {
        private readonly IJSRuntime _js;
        private bool _loaded;

        public FaceInteropService(IJSRuntime js)
        {
            _js = js;
        }

        public async Task EnsureLoadedAsync()
        {
            if (_loaded) return;
            await _js.InvokeVoidAsync("faceInterop.loadFaceApi");
            _loaded = true;
        }

        public async Task LoadModelsAsync(string modelBaseUrl)
        {
            await EnsureLoadedAsync();
            await _js.InvokeVoidAsync("faceInterop.loadModels", modelBaseUrl);
        }

        public async Task<FaceDetectionResult?> DetectSingleFaceEmbeddingAsync(string dataUrl)
        {
            await EnsureLoadedAsync();
            return await _js.InvokeAsync<FaceDetectionResult?>("faceInterop.detectSingleFaceEmbedding", dataUrl);
        }

        public async Task<List<FaceDetectionResult>> DetectMultipleFacesEmbeddingAsync(string dataUrl)
        {
            await EnsureLoadedAsync();
            return await _js.InvokeAsync<List<FaceDetectionResult>>("faceInterop.detectAllFacesEmbedding", dataUrl);
        }

        public async Task<List<FaceDetectionResult>> DetectMultipleFacesEmbeddingAsync(string dataUrl, int thumbnailMaxSize)
        {
            await EnsureLoadedAsync();
            return await _js.InvokeAsync<List<FaceDetectionResult>>("faceInterop.detectAllFacesEmbedding", dataUrl, thumbnailMaxSize);
        }
    }
}
