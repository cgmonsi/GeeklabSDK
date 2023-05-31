using System;
using UnityEngine;
using UnityEditor;
using System.Reflection;

public class SDKInfoEditorWindow : EditorWindow {
    private bool showAtStartup = true;
    private bool isModified;
    private bool missingValues;

    
    [MenuItem("GeekladSDK/SDK Info")]
    public static void ShowWindow() {
        GetWindow<SDKInfoEditorWindow>("SDK Info");
    }

    
    private void OnEnable() {
        showAtStartup = PlayerPrefs.GetString("ShowSDKInfoAtStartup", "true") == "True";
        SDKInfoChecker.CheckSDKInfoModel();
        isModified = false;
        missingValues = SDKInfoChecker.MissingValues;
    }
    
    
    private void OnDisable() {
        // SaveSDKInfoModel();
    }

    
    private void OnGUI() {
        if (missingValues) {
            EditorGUILayout.HelpBox("Not all fields are filled in.", MessageType.Warning);
        }
        
        EditorGUI.BeginChangeCheck();
        // showAtStartup = EditorGUILayout.Toggle("Show at startup", showAtStartup);
        EditorGUILayout.BeginVertical(GUI.skin.box); // Start of box for SDKInfoModel

        var sdkInfoModelProperties = typeof(SDKInfoModel).GetProperties(BindingFlags.Public | BindingFlags.Static);
        string currentGroup = null;
        foreach (var property in sdkInfoModelProperties) {
            var groupAttr = (FieldGroup)Attribute.GetCustomAttribute(property, typeof(FieldGroup));
            if (groupAttr != null && groupAttr.GroupName != currentGroup) {
                currentGroup = groupAttr.GroupName;
                EditorGUILayout.Space(); // Add space between groups
                EditorGUILayout.LabelField(currentGroup, EditorStyles.boldLabel); // Display group name
            }
            
            if (property.PropertyType == typeof(string)) {
                property.SetValue(null, EditorGUILayout.TextField(property.Name, (string)property.GetValue(null)));
            }
            else if (property.PropertyType == typeof(bool)) {
                property.SetValue(null, EditorGUILayout.Toggle(property.Name, (bool)property.GetValue(null)));
            }
        }
        
        EditorGUILayout.EndVertical(); // End of box for SDKInfoModel
        
        EditorGUILayout.Separator(); // separator line

        if (EditorGUI.EndChangeCheck()) {
            isModified = true;
            missingValues = false;
        }

        GUI.enabled = isModified;
        if (GUILayout.Button("Save")) {
            SaveSDKInfoModel();
            isModified = false;
        }
        GUI.enabled = true;
    }

    private void SaveSDKInfoModel() {
        var sdkInfoModelProperties = typeof(SDKInfoModel).GetProperties(BindingFlags.Public | BindingFlags.Static);
        foreach (var property in sdkInfoModelProperties) {
            PlayerPrefs.SetString(property.Name, property.GetValue(null).ToString());
            if (string.IsNullOrEmpty(property.GetValue(null).ToString()) || property.GetValue(null).ToString() == "0") {
                missingValues = true;
            }
        }
        PlayerPrefs.SetString("ShowSDKInfoAtStartup", showAtStartup.ToString());
        PlayerPrefs.Save();
    }
}