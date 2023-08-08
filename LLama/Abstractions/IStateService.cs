using LLama.Executors;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace LLama.Abstractions
{
    /// <summary>
    /// A high level interface for executor state loading saving.
    /// </summary>
    public interface IStateService<T> where T : ExecutorBaseState
    {
        /// <summary>
        /// Loads a executor state
        /// </summary>
        /// <returns></returns>
        T Load();

        /// <summary>
        /// Loads a executor state
        /// </summary>
        /// <returns></returns>
        bool Save(T state);

        /// <summary>
        /// Loads a executor state
        /// </summary>
        /// <returns></returns>
        Task<T> LoadAsync();

        /// <summary>
        /// Loads a executor state async
        /// </summary>
        /// <returns></returns>
        Task<bool> SaveAsync(T state);
    }
}
