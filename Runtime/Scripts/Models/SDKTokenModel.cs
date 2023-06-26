using UnityEngine;

namespace Kitrum.GeeklabSDK
{
    public class SDKTokenModel
    {
        // Singleton instance
        private static SDKTokenModel _instance;

        public static SDKTokenModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SDKTokenModel();
                }
                return _instance;
            }
        }

        // Token verification state
        public bool IsTokenVerified { get; set; }
        
        // SDK token
        public string Token { get; set; }

        
        public string GetToken()
        {
            return !PlayerPrefs.HasKey("SDKToken") ? null : PlayerPrefs.GetString("SDKToken");
        }
    }
}