#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.Purchasing;

[CustomEditor(typeof(SDKSettingsModel))]
public class SDKSettingsModelEditor : Editor
{
    private PurchasableItemListModel purchasableItems = new PurchasableItemListModel();


    public override void OnInspectorGUI() {
        SDKSettingsModel sdkSettings = (SDKSettingsModel)target;

        // Draw default Inspector
        DrawDefaultInspector();
        
        EditorGUILayout.Separator(); // separator line
        
        EditorGUILayout.BeginVertical(GUI.skin.box); // Start of box for PurchasableItems
        DrawPurchasableItems(sdkSettings);
        EditorGUILayout.EndVertical(); // End of box for PurchasableItems

        if (GUI.changed)
        {
            EditorUtility.SetDirty(sdkSettings);
            AssetDatabase.SaveAssets();
        }
    }

    private void DrawPurchasableItems(SDKSettingsModel sdkSettings) {
        EditorGUILayout.LabelField("Purchasable Items", EditorStyles.boldLabel);

        for (int i = 0; i < sdkSettings.purchasableItems.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            sdkSettings.purchasableItems[i].name = EditorGUILayout.TextField(sdkSettings.purchasableItems[i].name);
            sdkSettings.purchasableItems[i].type = (ProductType)EditorGUILayout.EnumPopup(sdkSettings.purchasableItems[i].type);
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
