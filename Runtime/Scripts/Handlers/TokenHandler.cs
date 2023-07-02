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

        private async void Awake()
        {
            var deepLink = DeepLinkHandler.CheckDeepLink();
            var clipboard = ClipboardHandler.GetText();
            var isVerifyCreativeToken = false;

            // If deep-link is present, read the creative token from it
            if (deepLink.Trim() != "")
            {
                var token = GetTokenFromText(deepLink.Trim());
                isVerifyCreativeToken = await VerifyCreativeToken(token);
                if (isVerifyCreativeToken)
                {
                    SetToken(token);
                    if (SDKSettingsModel.Instance.ShowDebugLog)
                        Debug.Log($"{SDKSettingsModel.GetColorPrefixLog()} Token from DeepLink = {GetCreativeToken()}");
                }
                else
                {
                    if (SDKSettingsModel.Instance.ShowDebugLog)
                        Debug.LogWarning($"{SDKSettingsModel.GetColorPrefixLog()} DeepLink Token: False");
                }
            }

            // // If deep-link is not present, try to read the creative token from clipboard
            if (clipboard.Trim() != "")
            {
                var token = GetTokenFromText(clipboard.Trim());
                isVerifyCreativeToken = await VerifyCreativeToken(token);

                if (isVerifyCreativeToken)
                {
                    SetToken(token);
                    if (SDKSettingsModel.Instance.ShowDebugLog)
                        Debug.Log($"{SDKSettingsModel.GetColorPrefixLog()} Token from Clipboard = {GetCreativeToken()}");
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
                var token = GetTokenFromText (await GetTokenFromGeeklab());
                isVerifyCreativeToken = ContainsToken(token);
                if (isVerifyCreativeToken)
                {
                    SetToken(token);
                    if (SDKSettingsModel.Instance.ShowDebugLog)
                        Debug.Log($"{SDKSettingsModel.GetColorPrefixLog()} Token from Clipboard = {GetCreativeToken()} - {token}");
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
                return null;
            
            var pattern = @"\b(\w+-)+\w+\b";
            var match = Regex.Match(input, pattern, RegexOptions.IgnoreCase);

            if (match.Success)
                return match.Value;
            else
                return null;
        }


        private static bool ContainsToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return false;

            var pattern =@"\b(\w+-)+\w+\b";

            if (Regex.IsMatch(token, pattern, RegexOptions.IgnoreCase))
                return true;
            else
                return false;
        }


        public static string GetCreativeToken()
        {
            return creativeToken;
        }


        public static void SetToken(string newToken)
        {
            creativeToken = newToken;
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
                    Debug.Log(response);

                    if (SDKSettingsModel.Instance.ShowDebugLog)
                        Debug.Log($"{response}");
                    taskCompletionSource.SetResult(true);
                },
                (error) =>
                {
                    Debug.LogError($"{error}");
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
                    if (SDKSettingsModel.Instance.ShowDebugLog)
                        Debug.Log($"{SDKSettingsModel.GetColorPrefixLog()} === {response}");
                    var tokenResponse = JsonUtility.FromJson<TokenResponseModel>(response);
                    SetToken(tokenResponse.token);
                    taskCompletionSource.SetResult(tokenResponse.token);
                },
                (error) =>
                {
                    // Debug.LogError($"{error}");
                    taskCompletionSource.SetResult(null);
                }
            );
            
            return await taskCompletionSource.Task;
        }
    }
}