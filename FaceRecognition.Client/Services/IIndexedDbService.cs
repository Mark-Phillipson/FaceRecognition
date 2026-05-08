using System.Collections.Generic;
using System.Threading.Tasks;

namespace FaceRecognition.Client.Services
{
    public interface IIndexedDbService
    {
        Task InitAsync();
        Task PutAsync<T>(string storeName, T item);
        Task<T> GetAsync<T>(string storeName, string key);
        Task<List<T>> GetAllAsync<T>(string storeName);
        Task DeleteAsync(string storeName, string key);
        Task ClearAsync(string storeName);
    }
}
