using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace LLama.Native
{
    /// <summary>
    /// Operating system information.
    /// </summary>
    /// <param name="OSPlatform"></param>
    /// <param name="CudaMajorVersion"></param>
    /// <param name="VulkanVersion"></param>
    public record SystemInfo(OSPlatform OSPlatform, int CudaMajorVersion, string? VulkanVersion)
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

            return new SystemInfo(platform, GetCudaMajorVersion(), GetVulkanVersion());
        }
        
        #region Vulkan version
        private static string? GetVulkanVersion()
        {
            // Get Vulkan Summary
            string? vulkanSummary = GetVulkanSummary();
            
            // If we have a Vulkan summary
            if (vulkanSummary != null)
            {
                // Extract Vulkan version from summary
                string? vulkanVersion = ExtractVulkanVersionFromSummary(vulkanSummary);
                
                // If we have a Vulkan version
                if (vulkanVersion != null)
                {
                    // Return the Vulkan version
                    return vulkanVersion;
                }
            }
            
            // Return null if we failed to get the Vulkan version
            return null;
        }
        
        private static string? GetVulkanSummary()
        {
            // Note: on Linux, this requires `vulkan-tools` to be installed. (`sudo apt install vulkan-tools`)
            try
            {
                // Start a process to read vulkan info
                Process process = new()
                {
                    StartInfo = new()
                    {
                        FileName = "vulkaninfo",
                        Arguments = "--summary",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                var (exitCode, output, error, ok) = process.SafeRun(TimeSpan.FromSeconds(12));

                // If ok return the output else return null 
                return ok ? output :
                    null;
            }
            catch
            {
                // Return null if we failed to get the Vulkan version
                return null;
            }
        }

        static string? ExtractVulkanVersionFromSummary(string vulkanSummary)
        {
            // We have three ways of parsing the Vulkan version from the summary (output is a different between Windows and Linux)
            // For now, I have decided to go with the full version number, and leave it up to the user to parse it further if needed
            // I have left the other patterns in, in case we need them in the future
            
            // Output on linux : 4206847 (1.3.255)
            // Output on windows : 1.3.255
            string pattern = @"apiVersion\s*=\s*([^\r\n]+)";
            
            // Output on linux : 4206847
            // Output on windows : 1.3.255
            //string pattern = @"apiVersion\s*=\s*([\d\.]+)";
            
            // Output on linux : 1.3.255
            // Output on windows : 1.3.255
            //string pattern = @"apiVersion\s*=\s*(?:\d+\s*)?(?:\(\s*)?([\d]+\.[\d]+\.[\d]+)(?:\s*\))?";
            
            // Create a Regex object to match the pattern
            Regex regex = new Regex(pattern);

            // Match the pattern in the input string
            Match match = regex.Match(vulkanSummary);

            // If a match is found
            if (match.Success)
            {
                // Return the version number
                return match.Groups[1].Value;
            }

            // Return null if no match is found
            return null;
        }
        #endregion

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
                // List of default cuda paths
                string[] defaultCudaPaths =
                [
                    "/usr/local/bin/cuda",
                    "/usr/local/cuda",
                ];
                
                // Loop through every default path to find the version
                foreach (var path in defaultCudaPaths)
                {
                    // Attempt to get the version from the path
                    version = GetCudaVersionFromPath(path);
                    
                    // If a CUDA version is found, break the loop
                    if (!string.IsNullOrEmpty(version))
                        break;
                }
                
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
