#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.Purchasing;

namespace Kitrum.GeeklabSDK
{
    [CustomEditor(typeof(SDKSettingsModel))]
    public class SDKSettingsModelEditor : Editor
    {
        private PurchasableItemListModel purchasableItems = new PurchasableItemListModel();
        public static bool isTokenVerified = false;
        
        private void OnEnable()
        {
            var sdkSettings = (SDKSettingsModel)target;
            VerifySDKToken(sdkSettings);
        }

        public override void OnInspectorGUI()
        {
            var sdkSettings = (SDKSettingsModel)target;

            // If the token is not verified, do not show the default inspector and purchasable items
            if (!SDKSettingsEditor.isTokenVerified)
            {
                EditorGUILayout.HelpBox("Token not verified. Please verify your token in the SDK Settings window.", MessageType.Warning);

                // Show the token as an input field
                sdkSettings.Token = EditorGUILayout.TextField("Token", sdkSettings.Token);
    
                // Button for token verification
                if (GUILayout.Button("Verify Token", GUILayout.Height(30), GUILayout.ExpandWidth(true)))
                {
                    VerifySDKToken(sdkSettings);
                    // // Your token verification logic here
                    // SDKSettingsEditor.isTokenVerified = SDKSettingsEditor.VerifySDKToken(sdkSettings.Token, sdkSettings);
                    // if(SDKSettingsEditor.isTokenVerified)
                    // {
                    //     EditorUtility.SetDirty(sdkSettings);
                    //     AssetDatabase.SaveAssets();
                    // }
                }
            }
            else
            {
                // Disable token field after verification
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField("Token", sdkSettings.Token);
                EditorGUI.EndDisabledGroup();

                // Button for clearing token
                if (GUILayout.Button("Clear Token", GUILayout.Height(30), GUILayout.ExpandWidth(true)))
                {
                    sdkSettings.Token = "";
                    SDKSettingsEditor.isTokenVerified = false;
                    EditorUtility.SetDirty(sdkSettings);
                    AssetDatabase.SaveAssets();
                }
                
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

        private static void VerifySDKToken(SDKSettingsModel sdkSettings)
        {
            SDKSettingsEditor.isTokenVerified = SDKSettingsEditor.VerifySDKToken(sdkSettings.Token, sdkSettings);
            if(SDKSettingsEditor.isTokenVerified)
            {
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