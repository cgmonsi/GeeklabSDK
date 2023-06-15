using System;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using UnityEngine.Purchasing;


namespace Kitrum.GeeklabSDK
{
    public class SDKSettingsEditor : EditorWindow
    {
        private static Vector2 minWindowSize = new Vector2(250, 300);
        
        private bool missingValues;
        public static bool isTokenVerified = false;
        private string tokenInputField = "";

        private SDKSettingsModel sdkSettings;

        [MenuItem("GeekladSDK/SDK Settings")]
        public static void ShowWindow()
        {
            GetWindow<SDKSettingsEditor>("SDK Settings").minSize = minWindowSize;
        }

        private void OnEnable()
        {
            var guids = AssetDatabase.FindAssets("t:SDKSettingsModel");
            if (guids.Length == 0)
            {
#if UNITY_EDITOR
                var instance = CreateInstance<SDKSettingsModel>();
                AssetDatabase.CreateAsset(instance, "Assets/Resources/SDKSettings.asset");
                AssetDatabase.SaveAssets();
                guids = AssetDatabase.FindAssets("t:SDKSettingsModel");
#else
            Debug.LogError("Could not find SDKSettings asset!");
            return;
#endif
            }

            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            sdkSettings = AssetDatabase.LoadAssetAtPath<SDKSettingsModel>(path);
            tokenInputField = sdkSettings.Token;

            // Automatically verify the token upon enabling
            if (!string.IsNullOrEmpty(sdkSettings.Token))
            {
                isTokenVerified = VerifySDKToken(sdkSettings.Token, sdkSettings);
            }
            
            missingValues = false;
        }

        private void OnGUI()
        {
            if (missingValues)
            {
                EditorGUILayout.HelpBox("Not all fields are filled in.", MessageType.Warning);
            }

            if (!isTokenVerified)
            {
                GUILayout.Label("Enter your SDK token:", EditorStyles.boldLabel);
                tokenInputField = EditorGUILayout.TextField("SDK Token", tokenInputField);
                if (GUILayout.Button("Verify Token", GUILayout.Height(30)))
                {
                    isTokenVerified = VerifySDKToken(tokenInputField, sdkSettings);
                    if (isTokenVerified)
                    {
                        sdkSettings.Token = tokenInputField;
                        SaveSDKSettingsModel();
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Error", "Invalid SDK token. Please try again.", "OK");
                    }
                }
            }
            else
            {
                if (GUILayout.Button("Clear SDK Token", GUILayout.Height(30)))
                {
                    sdkSettings.Token = "";
                    tokenInputField = "";
                    isTokenVerified = false;
                    SaveSDKSettingsModel();
                }
                
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.BeginVertical(GUI.skin.box); // Start of box for SDKSettingsModel
                DrawSDKSettingsModel();
                EditorGUILayout.EndVertical(); // End of box for SDKSettingsModel

                EditorGUILayout.Separator(); // separator line

                var isSDKDisabled = !sdkSettings.IsSDKEnabled || !sdkSettings.EnablePurchaseAnalytics; // New bool variable

                EditorGUI.BeginDisabledGroup(
                    isSDKDisabled); // Disable all controls within this group if isSDKDisabled == true
                EditorGUILayout.BeginVertical(GUI.skin.box); // Start of box for PurchasableItems
                DrawPurchasableItems();
                EditorGUILayout.EndVertical(); // End of box for PurchasableItems
                EditorGUI.EndDisabledGroup(); // End of disabled group

                if (EditorGUI.EndChangeCheck())
                {
                    missingValues = false;
                }
            }
        }

        private void DrawSDKSettingsModel()
        {
            var sdkSettingsModelFields =
                typeof(SDKSettingsModel).GetFields(BindingFlags.Public | BindingFlags.Instance);
            string currentGroup = null;
            foreach (var field in sdkSettingsModelFields)
            {
                var groupAttr = (FieldGroup)Attribute.GetCustomAttribute(field, typeof(FieldGroup));
                var hideAttr = (HideInFieldGroupAttribute)Attribute.GetCustomAttribute(field, typeof(HideInFieldGroupAttribute));
                if (hideAttr != null && hideAttr.GroupName == groupAttr.GroupName)
                {
                    continue; // Skip drawing this field
                }
                
                if (groupAttr != null && groupAttr.GroupName != currentGroup)
                {
                    currentGroup = groupAttr.GroupName;
                    EditorGUILayout.Space(); // Add space between groups
                    EditorGUILayout.LabelField(currentGroup, EditorStyles.boldLabel); // Display group name
                }

                var disableIfSDKDisabled = Attribute.IsDefined(field, typeof(DisableIfSDKDisabled)) &&
                                           !sdkSettings.IsSDKEnabled;
                var disableIfPurchaseAnalyticsDisabled =
                    Attribute.IsDefined(field, typeof(DisableIfPurchaseAnalyticsDisabled)) &&
                    !sdkSettings.EnablePurchaseAnalytics;
                var disableIfAdAnalyticsDisabled = Attribute.IsDefined(field, typeof(DisableIfAdAnalyticsDisabled)) &&
                                                   !sdkSettings.EnableAdAnalytics;

                var shouldDisable = disableIfSDKDisabled || disableIfPurchaseAnalyticsDisabled ||
                                    disableIfAdAnalyticsDisabled;

                EditorGUI.BeginDisabledGroup(shouldDisable);
                if (field.FieldType == typeof(string))
                {
                    var value = EditorGUILayout.TextField(field.Name, (string)field.GetValue(sdkSettings));
                    field.SetValue(sdkSettings, value);
                }
                else if (field.FieldType == typeof(bool))
                {
                    var value = EditorGUILayout.Toggle(field.Name, (bool)field.GetValue(sdkSettings));
                    field.SetValue(sdkSettings, value);
                }

                EditorGUI.EndDisabledGroup();
            }
        }

        
        public static bool VerifySDKToken(string token, SDKSettingsModel sdkSettings)
        {
            if (sdkSettings != null && token is "123")
            {
                sdkSettings.Token = token;
                return true;
            }
            return false;
        }

        private void DrawPurchasableItems()
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

        private void SaveSDKSettingsModel()
        {
            EditorUtility.SetDirty(sdkSettings);
            AssetDatabase.SaveAssets();
        }
    }
}