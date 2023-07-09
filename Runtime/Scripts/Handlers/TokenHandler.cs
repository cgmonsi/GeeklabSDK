using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using System.Text.RegularExpressions;


namespace Kitrum.GeeklabSDK
{
    public class TokenHandler : MonoBehaviour
    {
        private const string TOKEN_KEY = "GeeklabCreativeToken";
        private static string creativeToken = "";
        private static string lastCheckedToken = "";

        
        private void Start()
        {
            CheckToken();
        }
        
        
        private void OnApplicationFocus(bool hasFocus)
        {
            // if (hasFocus)
            //     CheckToken();
        }
        

        private async void CheckToken()
        {
            var deepLink = DeepLinkHandler.CheckDeepLink();
            if (deepLink == null) deepLink = "";
            var clipboard = ClipboardHandler.GetText();
            if (clipboard == null) clipboard = "";
            var isVerifyCreativeToken = false;

            if (!string.IsNullOrEmpty(GetCreativeToken().Trim()))
                return;

            // If deep-link is present, read the creative token from it
            if (ContainsToken(deepLink.Trim()))
            {
                var token = GetTokenFromText(deepLink.Trim());
                if (!lastCheckedToken.Equals(token))
                    isVerifyCreativeToken = await VerifyCreativeToken(token);

                lastCheckedToken = token;
                    
                if (isVerifyCreativeToken)
                {
                    SetToken(token);
                    if (SDKSettingsModel.Instance.ShowDebugLog)
                        Debug.Log($"{SDKSettingsModel.GetColorPrefixLog()} Token from DeepLink = {GetCreativeToken()}");
                    return;
                }
                else
                {
                    if (SDKSettingsModel.Instance.ShowDebugLog)
                        Debug.LogWarning($"{SDKSettingsModel.GetColorPrefixLog()} DeepLink Token: False");
                }
            }

            // // If deep-link is not present, try to read the creative token from clipboard
            if (SDKSettingsModel.Instance.CheckClipboardForToken && ContainsToken(clipboard.Trim()))
            {
                var token = GetTokenFromText(clipboard.Trim());
                if (!lastCheckedToken.Equals(token))
                    isVerifyCreativeToken = await VerifyCreativeToken(token);

                lastCheckedToken = token;
                
                if (isVerifyCreativeToken)
                {
                    SetToken(token);
                    if (SDKSettingsModel.Instance.ShowDebugLog)
                        Debug.Log($"{SDKSettingsModel.GetColorPrefixLog()} Token from Clipboard = {GetCreativeToken()}");
                    return;
                }
                else
                {
                    if (SDKSettingsModel.Instance.ShowDebugLog)
                        Debug.LogWarning($"{SDKSettingsModel.GetColorPrefixLog()} Clipboard Token: False");
                }
            }

            // If there is still no token, get one from Geeklab endpoint
            if (GetCreativeToken() == "")
            {
                var token = await GetTokenFromGeeklab();

                if (!lastCheckedToken.Equals(token))
                    isVerifyCreativeToken = !string.IsNullOrEmpty(token);
                
                lastCheckedToken = token;
                
                if (isVerifyCreativeToken)
                {
                    SetToken(token);
                    if (SDKSettingsModel.Instance.ShowDebugLog)
                        Debug.Log($"{SDKSettingsModel.GetColorPrefixLog()} Token from Geeklab = {GetCreativeToken()}");
                }
                else
                {
                    if (SDKSettingsModel.Instance.ShowDebugLog)
                        Debug.LogWarning($"{SDKSettingsModel.GetColorPrefixLog()} Geeklab Token: False");
                }
            }
        }
        
        
        private static string GetTokenFromText(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";
    
            var pattern = @"(?:.*:\/\/.*\?geeklab_ct:|\bgeeklab_ct:)\s*(\w+)\s*";
            var match = Regex.Match(input, pattern);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return "";
        }


        private static bool ContainsToken(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            var pattern = @"(?:.*:\/\/.*\?geeklab_ct:|\bgeeklab_ct:)\s*(\w+)\s*";
            var match = Regex.Match(input, pattern);

            return match.Success;
        }


        public static string GetCreativeToken()
        {
            return creativeToken;
        }


        private static void SetToken(string newToken)
        {
            creativeToken = newToken.TrimStart('?');
            SaveTokenLocally();
        }


        private static void SaveTokenLocally()
        {
            PlayerPrefs.SetString(TOKEN_KEY, creativeToken);
            PlayerPrefs.Save();
        }

        
        private static async Task<bool> VerifyCreativeToken(string token)
        {
            if (string.IsNullOrEmpty(token) || !SDKSettingsModel.Instance.SendStatistics) return false;
            
            var taskCompletionSource = new TaskCompletionSource<bool>();

            WebRequestManager.Instance.VerifyCreativeTokenRequest(token,
                (response) =>
                {
                    // if (SDKSettingsModel.Instance.ShowDebugLog)
                    //     Debug.Log($"{response}");
                    taskCompletionSource.SetResult(true);
                },
                (error) =>
                {
                    // Debug.LogWarning($"{error}");
                    taskCompletionSource.SetResult(false);
                }
            );
            
            return await taskCompletionSource.Task;
        }
        

        public static async Task<string> GetTokenFromGeeklab()
        {
            if (!SDKSettingsModel.Instance.SendStatistics)
                return null;
            
            var taskCompletionSource = new TaskCompletionSource<string>();

            WebRequestManager.Instance.FetchTokenRequest(
                (response) =>
                {
                    // if (SDKSettingsModel.Instance.ShowDebugLog)
                    //     Debug.Log($"{SDKSettingsModel.GetColorPrefixLog()} {response}");
                    var tokenResponse = JsonUtility.FromJson<TokenResponseModel>(response);
                    SetToken(tokenResponse.token);
                    taskCompletionSource.SetResult(tokenResponse.token);
                },
                (error) =>
                {
                    Debug.LogError($"{error}");
                    taskCompletionSource.SetResult(null);
                }
            );
            
            return await taskCompletionSource.Task;
        }
    }
}