using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace FaceRecognition.Client.Services
{
    public class IndexedDbService : IIndexedDbService
    {
        private readonly IJSRuntime _js;
        private const string DbName = "FaceRecognitionDb";
        private const int DbVersion = 1;

        public IndexedDbService(IJSRuntime js)
        {
            _js = js;
        }

        public async Task InitAsync()
        {
            await _js.InvokeVoidAsync("indexedDbInterop.init", DbName, DbVersion);
        }

        public async Task PutAsync<T>(string storeName, T item)
        {
            await _js.InvokeVoidAsync("indexedDbInterop.put", DbName, storeName, item);
        }

        public async Task<T> GetAsync<T>(string storeName, string key)
        {
            return await _js.InvokeAsync<T>("indexedDbInterop.get", DbName, storeName, key);
        }

        public async Task<List<T>> GetAllAsync<T>(string storeName)
        {
            return await _js.InvokeAsync<List<T>>("indexedDbInterop.getAll", DbName, storeName);
        }

        public async Task DeleteAsync(string storeName, string key)
        {
            await _js.InvokeVoidAsync("indexedDbInterop.delete", DbName, storeName, key);
        }

        public async Task ClearAsync(string storeName)
        {
            await _js.InvokeVoidAsync("indexedDbInterop.clear", DbName, storeName);
        }
    }
}
