using UnityEngine;
using System;
using System.Runtime.InteropServices;

namespace Kitrum.GeeklabSDK
{
    public class DeepLinkHandler : MonoBehaviour
    {
        private static string deepLink;


        public static string CheckDeepLink()
        {
            deepLink = Application.absoluteURL;
            // InitTestDeepLinking();

            if (string.IsNullOrEmpty(deepLink))
            {
                return "";
            }

            return deepLink;
        }
    

        public static string GetDeepLink()
        {
            return deepLink;
        }

        private static void InitTestDeepLinking()
        {
            deepLink = "App://web/path?creative_token=test_token";
        }
    }
}