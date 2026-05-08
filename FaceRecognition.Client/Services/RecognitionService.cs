using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FaceRecognition.Client.Models;

namespace FaceRecognition.Client.Services
{
    public class RecognitionService : IRecognitionService
    {
        private readonly IIndexedDbService _db;
        private readonly IFaceService _faceService;

        public RecognitionService(IIndexedDbService db, IFaceService faceService)
        {
            _db = db;
            _faceService = faceService;
        }

        public async Task ComputeEmbeddingsForAllImagesAsync()
        {
            var images = await _db.GetAllAsync<FaceImage>("images");
            var embeddings = await _db.GetAllAsync<FaceEmbedding>("embeddings");
            var existingImageIds = new HashSet<string>(embeddings?.Select(e => e.ImageId) ?? Enumerable.Empty<string>());

            foreach (var image in images)
            {
                if (existingImageIds.Contains(image.Id)) continue;
                try
                {
                    var detection = await _faceService.DetectSingleFaceEmbeddingAsync(image.DataUri);
                    if (detection != null && detection.Descriptor?.Length > 0)
                    {
                        var embedding = new FaceEmbedding
                        {
                            PersonId = image.PersonId,
                            ImageId = image.Id,
                            Embedding = detection.Descriptor
                        };
                        await _db.PutAsync("embeddings", embedding);
                    }
                }
                catch
                {
                    // ignore individual failures and continue
                }
            }
        }

        public async Task<List<MatchResult>> FindMatchesAsync(float[] queryEmbedding, int topN = 5)
        {
            var embeddings = await _db.GetAllAsync<FaceEmbedding>("embeddings");
            var images = await _db.GetAllAsync<FaceImage>("images");
            var persons = await _db.GetAllAsync<Person>("persons");
            var personMap = (persons ?? new List<Person>()).ToDictionary(p => p.Id, p => p);

            var results = new List<MatchResult>();
            if (embeddings == null || embeddings.Count == 0) return results;

            foreach (var emb in embeddings)
            {
                if (emb.Embedding == null || emb.Embedding.Length == 0) continue;
                var distance = EuclideanDistance(queryEmbedding, emb.Embedding);
                results.Add(new MatchResult
                {
                    EmbeddingId = emb.Id,
                    ImageId = emb.ImageId,
                    PersonId = emb.PersonId,
                    PersonName = personMap.ContainsKey(emb.PersonId) ? personMap[emb.PersonId].Name : string.Empty,
                    Distance = distance,
                    ImageDataUrl = images?.FirstOrDefault(i => i.Id == emb.ImageId)?.DataUri ?? string.Empty
                });
            }

            return results.OrderBy(r => r.Distance).Take(topN).ToList();
        }

        private static double EuclideanDistance(float[] a, float[] b)
        {
            double sum = 0;
            var len = Math.Min(a.Length, b.Length);
            for (int i = 0; i < len; i++)
            {
                var d = a[i] - b[i];
                sum += d * d;
            }
            return Math.Sqrt(sum);
        }
    }
}
