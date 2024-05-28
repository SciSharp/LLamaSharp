using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace LLama.Native
{
    /// <summary>
    /// Operating system information.
    /// </summary>
    /// <param name="OSPlatform"></param>
    /// <param name="CudaMajorVersion"></param>
    public record class SystemInfo(OSPlatform OSPlatform, int CudaMajorVersion)
    {
        /// <summary>
        /// Get the system information of the current machine.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="PlatformNotSupportedException"></exception>
        public static SystemInfo Get()
        {
            OSPlatform platform;
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                platform = OSPlatform.Windows;
            }
            else if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                platform = OSPlatform.Linux;
            }
            else if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                platform = OSPlatform.OSX;
            }
            else
            {
                throw new PlatformNotSupportedException();
            }

            return new SystemInfo(platform, GetCudaMajorVersion());
        }

        #region CUDA version
        private static int GetCudaMajorVersion()
        {
            string? cudaPath;
            string version = "";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                cudaPath = Environment.GetEnvironmentVariable("CUDA_PATH");
                if (cudaPath is null)
                {
                    return -1;
                }

                //Ensuring cuda bin path is reachable. Especially for MAUI environment.
                string cudaBinPath = Path.Combine(cudaPath, "bin");

                if (Directory.Exists(cudaBinPath))
                {
                    AddDllDirectory(cudaBinPath);
                }

                version = GetCudaVersionFromPath(cudaPath);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // Try the default first
                cudaPath = "/usr/local/bin/cuda";
                version = GetCudaVersionFromPath(cudaPath);
                if (string.IsNullOrEmpty(version))
                {
                    cudaPath = Environment.GetEnvironmentVariable("LD_LIBRARY_PATH");
                    if (cudaPath is null)
                    {
                        return -1;
                    }
                    foreach (var path in cudaPath.Split(':'))
                    {
                        version = GetCudaVersionFromPath(Path.Combine(path, ".."));
                        if (string.IsNullOrEmpty(version))
                        {
                            break;
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(version))
                return -1;

            version = version.Split('.')[0];
            if (int.TryParse(version, out var majorVersion))
                return majorVersion;

            return -1;
        }

        private static string GetCudaVersionFromPath(string cudaPath)
        {
            try
            {
                string json = File.ReadAllText(Path.Combine(cudaPath, cudaVersionFile));
                using (JsonDocument document = JsonDocument.Parse(json))
                {
                    JsonElement root = document.RootElement;
                    JsonElement cublasNode = root.GetProperty("libcublas");
                    JsonElement versionNode = cublasNode.GetProperty("version");
                    if (versionNode.ValueKind == JsonValueKind.Undefined)
                    {
                        return string.Empty;
                    }
                    return versionNode.GetString() ?? "";
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        // Put it here to avoid calling NativeApi when getting the cuda version.
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int AddDllDirectory(string NewDirectory);

        private const string cudaVersionFile = "version.json";
        #endregion
    }
}
