using UnityEngine;
using UnityEditor;
using System.Reflection;


[InitializeOnLoad]
public class SDKInfoChecker {
    public static bool MissingValues { get; private set; }

    static SDKInfoChecker() {
        EditorApplication.playModeStateChanged += LogPlayModeState;

        var sdkInfoModelProperties = typeof(SDKInfoModel).GetProperties(BindingFlags.Public | BindingFlags.Static);
        MissingValues = false;
        foreach (var property in sdkInfoModelProperties) {
            if (!PlayerPrefs.HasKey(property.Name) || string.IsNullOrEmpty(PlayerPrefs.GetString(property.Name)) || PlayerPrefs.GetString(property.Name) == "0") {
                MissingValues = true;
                break;
            }
        }

        if (MissingValues) { // && PlayerPrefs.GetString("ShowSDKInfoAtStartup", "true") == "True"
            SDKInfoEditorWindow.ShowWindow();
        }
    }

    
    private static void LogPlayModeState(PlayModeStateChange state) {
        if (state == PlayModeStateChange.ExitingEditMode || state == PlayModeStateChange.EnteredPlayMode) {
            CheckSDKInfoModel();
        }
    }

    
    public static void CheckSDKInfoModel() {
        var sdkInfoModelProperties = typeof(SDKInfoModel).GetProperties(BindingFlags.Public | BindingFlags.Static);
        foreach (var property in sdkInfoModelProperties) {
            if (!PlayerPrefs.HasKey(property.Name)) {
                // Debug.LogWarning("Missing value for SDKInfoModel property: " + property.Name);
                if (property.PropertyType == typeof(string)) {
                    property.SetValue(null, property.GetValue(""));
                }
                else if (property.PropertyType == typeof(bool)) {
                    property.SetValue(null, (bool)property.GetValue(true));
                }
                PlayerPrefs.SetString(property.Name, property.GetValue(null).ToString());
            }
            else {
                if (property.PropertyType == typeof(string)) {
                    property.SetValue(null, PlayerPrefs.GetString(property.Name));
                }
                else if (property.PropertyType == typeof(bool)) {
                    property.SetValue(null, PlayerPrefs.GetString(property.Name) == "True");
                }
            }
        }
    }
}