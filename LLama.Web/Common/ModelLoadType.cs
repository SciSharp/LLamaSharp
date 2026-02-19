namespace LLama.Web.Common
{
    /// <summary>
    /// The type of model load caching to use.
    /// </summary>
    public enum ModelLoadType
    {

        /// <summary>
        /// Only one model is loaded into memory at a time; any other models are unloaded before the new one is loaded.
        /// </summary>
        Single = 0,

        /// <summary>
        /// Multiple models are loaded into memory; ensure you use the ModelConfigs to split hardware resources.
        /// </summary>
        Multiple = 1,

        /// <summary>
        /// The first model in the appsettings.json list is preloaded into memory at app startup.
        /// </summary>
        PreloadSingle = 2,


        /// <summary>
        /// All models in the appsettings.json list are preloaded into memory at app startup; ensure you use the ModelConfigs to split hardware resources.
        /// </summary>
        PreloadMultiple = 3,
    }
}
