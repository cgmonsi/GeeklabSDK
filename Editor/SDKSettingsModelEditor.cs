#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Purchasing;

namespace Kitrum.GeeklabSDK
{
    [CustomEditor(typeof(SDKSettingsModel))]
    public class SDKSettingsModelEditor : Editor
    {
        private PurchasableItemListModel purchasableItems = new PurchasableItemListModel();
        private static SDKSettingsModelEditor _instance;

        private SDKSettingsModelEditor()
        {
        }
        
        
        public static SDKSettingsModelEditor GetInstance()
        {
            if (_instance == null)
            {
                _instance = new SDKSettingsModelEditor();
            }

            return _instance;
        }

        private void OnEnable()
        {
            var sdkSettings = (SDKSettingsModel)target;
            if (PlayerPrefs.HasKey("SDKToken"))
            {
                sdkSettings.Token = PlayerPrefs.GetString("SDKToken");
                SDKTokenModel.Instance.Token = sdkSettings.Token;
                SDKTokenModel.Instance.IsTokenVerified = true;
            }
            // VerifySDKToken(sdkSettings);
        }

        public override void OnInspectorGUI()
        {
            var sdkSettings = (SDKSettingsModel)target;

            // If the token is not verified, do not show the default inspector and purchasable items
            if (!SDKTokenModel.Instance.IsTokenVerified)
            {
                EditorGUILayout.HelpBox("Token not verified. Please verify your token in the SDK Settings window.", MessageType.Warning);

                // Show the token as an input field
                sdkSettings.Token = EditorGUILayout.TextField("Token", sdkSettings.Token);

                // Button for token verification
                if (GUILayout.Button("Verify Token", GUILayout.Height(30), GUILayout.ExpandWidth(true)))
                {
                    VerifySDKToken(sdkSettings);
                    Repaint();
                    GUI.FocusControl(null);
                }
            }
            else
            {
                // Button for clearing token
                if (GUILayout.Button("Clear Token", GUILayout.Height(30), GUILayout.ExpandWidth(true)))
                {
                    sdkSettings.Token = "";
                    SDKTokenModel.Instance.IsTokenVerified = false;
                    SDKTokenModel.Instance.Token = "";
                    PlayerPrefs.DeleteKey("SDKToken");
                    PlayerPrefs.Save();
                    EditorUtility.SetDirty(sdkSettings);
                    AssetDatabase.SaveAssets();
                    GUI.FocusControl(null);
                    Repaint();
                }
                
                // Disable token field after verification
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField("Token", sdkSettings.Token);
                EditorGUI.EndDisabledGroup();
                
                // Draw default Inspector
                DrawDefaultInspector();

                EditorGUILayout.Separator(); // separator line

                EditorGUILayout.BeginVertical(GUI.skin.box); // Start of box for PurchasableItems
                DrawPurchasableItems(sdkSettings);
                EditorGUILayout.EndVertical(); // End of box for PurchasableItems
            }
        
            if (GUI.changed)
            {
                EditorUtility.SetDirty(sdkSettings);
                AssetDatabase.SaveAssets();
            }
        }

        private static async void VerifySDKToken(SDKSettingsModel sdkSettings)
        {
            if (string.IsNullOrEmpty(sdkSettings.Token)) // check if the token field is empty
            {
                return;
            }
    
            var tcs = new TaskCompletionSource<bool>();
            var www = UnityWebRequest.Get(ApiEndpointsModel.VERIFY_API_KEY);
            www.SetRequestHeader("Authorization", "Bearer " + sdkSettings.Token);

            www.SendWebRequest().completed += _ => tcs.SetResult(true);

            await tcs.Task;

#if UNITY_2020_2_OR_NEWER
            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
#else
            if (www.isNetworkError || www.isHttpError)
#endif
            {
                EditorUtility.DisplayDialog("Error", "Invalid SDK token. Please try again.", "OK");
                SDKTokenModel.Instance.IsTokenVerified = false;
                SDKTokenModel.Instance.Token = "";
            }
            else
            {
                SDKTokenModel.Instance.IsTokenVerified = true;
                SDKTokenModel.Instance.Token = sdkSettings.Token;
                PlayerPrefs.SetString("SDKToken", sdkSettings.Token);
                PlayerPrefs.Save();
                EditorUtility.SetDirty(sdkSettings);
                AssetDatabase.SaveAssets();
            }
        }


        private void DrawPurchasableItems(SDKSettingsModel sdkSettings)
        {
            EditorGUILayout.LabelField("Purchasable Items", EditorStyles.boldLabel);

            for (int i = 0; i < sdkSettings.purchasableItems.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                sdkSettings.purchasableItems[i].name = EditorGUILayout.TextField(sdkSettings.purchasableItems[i].name);
                sdkSettings.purchasableItems[i].type =
                    (ProductType)EditorGUILayout.EnumPopup(sdkSettings.purchasableItems[i].type);
                if (GUILayout.Button("Remove"))
                {
                    sdkSettings.purchasableItems.RemoveAt(i);
                }

                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Item"))
            {
                sdkSettings.purchasableItems.Add(new PurchasableItemModel());
            }
        }
    }
}