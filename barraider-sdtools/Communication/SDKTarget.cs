namespace BarRaider.SdTools.Communication
{
    /// <summary>
    /// Target to send Title/Image to
    /// </summary>
    public enum SDKTarget : int
    {
        /// <summary>
        /// Send to both App and Device
        /// </summary>
        HardwareAndSoftware = 0,

        /// <summary>
        /// Send only to device
        /// </summary>
        HardwareOnly = 1,

        /// <summary>
        /// Send only to app
        /// </summary>
        SoftwareOnly = 2,
    }
}
