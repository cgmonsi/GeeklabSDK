using UnityEngine;
using System;
using System.Runtime.InteropServices;

namespace Kitrum.GeeklabSDK
{
    public class ClipboardHandler : MonoBehaviour
    {
#if UNITY_IOS

    public static string ReadClipboard()
    {
        return ClipboardManager.GetText();
    }

#elif UNITY_ANDROID
    public static void WriteToClipboard(string str)
    {
        // AndroidJavaClass ClipboardClass = new AndroidJavaClass("android.content.ClipboardManager");
        // AndroidJavaObject clipboardInstance = ClipboardClass.CallStatic<AndroidJavaObject>("getInstance", Context);
        // clipboardInstance.Call("setText", str);
    }

    public static string ReadClipboard()
    {
        // AndroidJavaClass ClipboardClass = new AndroidJavaClass("android.content.ClipboardManager");
        // AndroidJavaObject clipboardInstance = ClipboardClass.CallStatic<AndroidJavaObject>("getInstance", Context);
        // return clipboardInstance.Call<string>("getText");
        return "";
    }
#else
        public static void WriteToClipboard(string str)
        {
            var te = new TextEditor
            {
                text = str
            };
            te.SelectAll();
            te.Copy();
        }

        public static string ReadClipboard()
        {
            var te = new TextEditor();
            te.Paste();
            return te.text.Trim();
        }

#endif
    }
}