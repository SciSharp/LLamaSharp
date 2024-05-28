namespace LLama.Web.Common
{
    /// <summary>
    /// The type of model load caching to use
    /// </summary>
    public enum ModelLoadType
    {

        /// <summary>
        /// Only one model will be loaded into memory at a time, any other models will be unloaded before the new one is loaded
        /// </summary>
        Single = 0,

        /// <summary>
        /// Multiple models will be loaded into memory, ensure you use the ModelConfigs to split the hardware resources
        /// </summary>
        Multiple = 1,

        /// <summary>
        /// The first model in the appsettings.json list will be preloaded into memory at app startup
        /// </summary>
        PreloadSingle = 2,


        /// <summary>
        /// All models in the appsettings.json list will be preloaded into memory at app startup, ensure you use the ModelConfigs to split the hardware resources
        /// </summary>
        PreloadMultiple = 3,
    }
}
