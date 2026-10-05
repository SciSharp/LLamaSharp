using System;
using System.Collections.Generic;

namespace LLama.Exceptions;

/// <summary>
/// Thrown when a device name in <see cref="Abstractions.IModelParams.Devices"/> does not match any available ggml backend device
/// </summary>
public class UnknownDeviceException
    : Exception
{
    /// <summary>
    /// The device name which was requested but could not be found
    /// </summary>
    public string RequestedDevice { get; }

    /// <summary>
    /// Names of all devices available on this machine, as returned by <see cref="Native.NativeApi.ggml_backend_dev_name"/>
    /// </summary>
    public IReadOnlyList<string> AvailableDevices { get; }

    /// <summary>
    /// Create a new UnknownDeviceException
    /// </summary>
    /// <param name="requestedDevice">The device name which was requested but could not be found</param>
    /// <param name="availableDevices">Names of all devices available on this machine</param>
    public UnknownDeviceException(string requestedDevice, IReadOnlyList<string> availableDevices)
        : base($"Unknown device '{requestedDevice}'. Available devices: {string.Join(", ", availableDevices)}")
    {
        RequestedDevice = requestedDevice;
        AvailableDevices = availableDevices;
    }
}
