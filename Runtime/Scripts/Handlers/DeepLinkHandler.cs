using UnityEngine;

public class DeepLinkHandler : MonoBehaviour
{
    private static string deepLink;
    

    public static void CheckDeepLink()
    {
        deepLink = Application.absoluteURL;
        // InitTestDeepLinking();
        
        if (string.IsNullOrEmpty(deepLink)) {
            return;
        }

        var creativeToken = ParseDeepLink(deepLink);

        if (!string.IsNullOrEmpty(creativeToken))
        {
            TokenHandler.SetToken(creativeToken);
        }
    }
    
    private static string ParseDeepLink(string deepLink)
    {
        var regex = new System.Text.RegularExpressions.Regex(@"[=\/\\]([^=\/\\]*)$");
        var match = regex.Match(deepLink);
        return match.Success ? match.Groups[1].Value : "";
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