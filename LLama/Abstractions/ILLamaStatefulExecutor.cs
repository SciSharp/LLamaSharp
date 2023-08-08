using System.Threading.Tasks;

namespace LLama.Abstractions
{  /// <summary>
   /// A high level interface for LLama models with state saving functions
   /// </summary>
    public interface ILLamaStatefulExecutor : ILLamaExecutor
    {
        /// <summary>
        /// Load executor state
        /// </summary>
        void LoadState();

        /// <summary>
        /// Save executor state
        /// </summary>
        bool SaveState();
        
         /// <summary>
        /// Load executor state async
        /// </summary>
        Task LoadStateAsync();

        /// <summary>
        /// Save executor state async
        /// </summary>
        Task<bool> SaveStateAsync();
      
    }
}
