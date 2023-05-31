using System;
using System.Linq;
using UnityEngine;

public class TokenHandler : MonoBehaviour
{
    private const string TOKEN_KEY = "GeeklabToken";
    private static string creativeToken = "";
    
    private void Awake() {
        // If deep-link is present, read the creative token from it
        DeepLinkHandler.CheckDeepLink();
        
        // If deep-link is not present, try to read the creative token from clipboard
        if (GetToken() == "")
        {
            var clipboard = ClipboardHandler.ReadClipboard();
            SetToken(clipboard);
        }
        
        // // If there is still no token, get one from Geeklab endpoint
        // if (GetToken() == "")
        // {
        //     GetTokenFromGeeklab();
        // }
        
        // If there is still no token, just create it
        if (GetToken() == "")
        {
            CreateNewToken();
        }

        // Debug.Log($"Token = {GetToken()}");
    }
    
    
    private static void CreateNewToken()
    {
        if (PlayerPrefs.GetString(TOKEN_KEY, "") == "")
        {
            creativeToken = GenerateToken();
            PlayerPrefs.SetString(TOKEN_KEY, creativeToken);
        }
        else
        {
            creativeToken = PlayerPrefs.GetString(TOKEN_KEY);
        }

        SaveTokenLocally();
    }

    
    public static string GetToken()
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

    
    private static string GenerateToken()
    {
        var time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
        var key = Guid.NewGuid().ToByteArray();
        var token = Convert.ToBase64String(time.Concat(key).ToArray());

        return token;
    }
    
    
    
    public static void GetTokenFromGeeklab()
    {
        WebRequestManager.Instance.GetTokenRequest(
            (response) => {
                Debug.Log("tokenResponse: " + response);
                var tokenResponse = JsonUtility.FromJson<TokenResponse>(response);
                TokenHandler.SetToken(tokenResponse.token);
            },
            (error) => {
                Debug.LogError("Error: " + error);
            }
        );
    }
}