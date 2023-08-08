using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using LLama.Abstractions;

namespace LLama.Executors
{
    /// <summary>
    /// Servive for saving executor state to a json file
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class JsonFileStateService<T> : IStateService<T> where T: ExecutorBaseState
    {
        private readonly string? _fileName;
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename">file to load and save</param>
        public JsonFileStateService(string? filename)
        {
            _fileName = filename;
        }


        /// <inheritdoc/>
        public T Load()
        {
            if (string.IsNullOrEmpty(_fileName))
                return default;

            using (var fs = new FileStream(_fileName, FileMode.Open, FileAccess.Read))
            {
                return JsonSerializer.Deserialize<T>(fs);
            }
        }


        /// <inheritdoc/>
        public bool Save(T state)
        {
            if (string.IsNullOrEmpty(_fileName))
                return false;

            using (var fs = new FileStream(_fileName, FileMode.Create, FileAccess.Write))
            {
                JsonSerializer.Serialize(fs, state);
            }
            return true;
        }


        /// <inheritdoc/>
        public async Task<T> LoadAsync()
        {
            if (string.IsNullOrEmpty(_fileName))
                return default;

            using (var fs = new FileStream(_fileName, FileMode.Open, FileAccess.Read))
            {
                return await JsonSerializer.DeserializeAsync<T>(fs);
            }
        }


        /// <inheritdoc/>
        public async Task<bool> SaveAsync(T state)
        {
            if (string.IsNullOrEmpty(_fileName))
                return false;

            using (var fs = new FileStream(_fileName, FileMode.Create, FileAccess.Write))
            {
                await JsonSerializer.SerializeAsync(fs, state);
            }
            return true;
        }
    }
}
