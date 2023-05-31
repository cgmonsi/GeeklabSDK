// using System.Collections;
// using System.Collections.Generic;
// using UnityEditor;
// using UnityEngine;
//
// [CustomEditor(typeof(GeeklabSDK))]
// public class GeeklabSDKEditor : Editor
// {
//     public override void OnInspectorGUI()
//     {
//         var geeklabSDK = (GeeklabSDK)target;
//
//         EditorGUILayout.BeginHorizontal();
//         var labelWidth = EditorGUIUtility.currentViewWidth * 1.0f;
//         EditorGUILayout.LabelField("Show Service in Hierarchy", GUILayout.MaxWidth(labelWidth));
//         geeklabSDK.ShowServiceInHierarchy = EditorGUILayout.Toggle(geeklabSDK.ShowServiceInHierarchy);
//         EditorGUILayout.EndHorizontal();
//
//     }
// }