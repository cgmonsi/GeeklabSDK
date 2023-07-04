using System;

namespace Kitrum.GeeklabSDK
{
    [Serializable]
    public class DeviceInfoModel
    {
        public float Dpi;
        public int Width;
        public int Height;
        public bool LowPower;
        public string Timezone;
        public string IosSystem;
        public string DeviceName;
        public string DeviceType;
        public string Generation;
        public string DeviceModel;
        public string GraphicsDeviceID;
        public string GraphicsDeviceVendor;
        public string GraphicsDeviceVersion;
        public string SessionStartTime;
        public int SessionDurationInSeconds;
        public string[] Resolutions;
        public string[] InstalledFonts;
    }
}