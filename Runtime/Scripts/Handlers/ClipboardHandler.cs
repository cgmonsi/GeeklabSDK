using UnityEngine;
using System.Runtime.InteropServices;
using System.Reflection;
using System;

public class ClipboardHandler: MonoBehaviour
{
    static IBoard _board;
    static IBoard board{
        get{
            if (_board == null) {
                #if UNITY_ANDROID && !UNITY_EDITOR
                _board = new AndroidBoard();
                #elif UNITY_IOS && !UNITY_TVOS && !UNITY_EDITOR
                _board = new IOSBoard ();
                #else
                _board = new StandardBoard(); 
                #endif
            }
            return _board;
        }
    }

    public static void SetText(string str){
        board.SetText (str);
    }

    public static string GetText(){
        return board.GetText ();
    }
}

interface IBoard{
    void SetText(string str);
    string GetText();
}

class StandardBoard : IBoard {
    private static PropertyInfo m_systemCopyBufferProperty = null;
    private static PropertyInfo GetSystemCopyBufferProperty() {
        if (m_systemCopyBufferProperty == null) {
            Type T = typeof(GUIUtility);
            m_systemCopyBufferProperty = T.GetProperty("systemCopyBuffer", BindingFlags.Static | BindingFlags.Public);
            if (m_systemCopyBufferProperty == null)
            {
                m_systemCopyBufferProperty =
                    T.GetProperty("systemCopyBuffer", BindingFlags.Static | BindingFlags.NonPublic);
            }

            if (m_systemCopyBufferProperty == null)
            {
                throw new Exception(
                    "Can't access internal member 'GUIUtility.systemCopyBuffer' it may have been removed / renamed");
            }
        }
        return m_systemCopyBufferProperty;
    }
    public void SetText(string str) {
        PropertyInfo P = GetSystemCopyBufferProperty();
        P.SetValue(null, str, null);
    }
    public string GetText(){
        PropertyInfo P = GetSystemCopyBufferProperty();
        return (string)P.GetValue(null, null);
    }
}

#if UNITY_IOS && !UNITY_TVOS
class IOSBoard : IBoard {
    [DllImport("__Internal")]
    static extern void SetText_ (string str);
    [DllImport("__Internal")]
    static extern string GetText_();

    public void SetText(string str){
        if (Application.platform != RuntimePlatform.OSXEditor) {
            SetText_ (str);
        }
    }

    public string GetText(){
        return GetText_();
    }
}
#endif

#if UNITY_ANDROID
class AndroidBoard : IBoard
{
    private AndroidJavaObject activity;

    public AndroidBoard() 
    {
        var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    }
    
    
   public void SetText(string str)
    {
        using (var clipData = new AndroidJavaObject("android.content.ClipData", new object[]{ str, str, new AndroidJavaObject("android.content.ClipData$Item", str)}))
        {
            GetClipboardManager().Call("setPrimaryClip", clipData);
        }
    }

    public string GetText()
    {
        using (var clipData = GetClipboardManager().Call<AndroidJavaObject>("getPrimaryClip"))
        {
            if (clipData.Call<int>("getItemCount") > 0)
            {
                return clipData.Call<AndroidJavaObject>("getItemAt", 0).Call<string>("coerceToText", activity).ToString();
            }
        }
        return null;
    }

    AndroidJavaObject GetClipboardManager()
    {
        var staticContext = new AndroidJavaClass("android.content.Context");
        var service = staticContext.GetStatic<string>("CLIPBOARD_SERVICE");
        return activity.Call<AndroidJavaObject>("getSystemService", service);
    }
}
#endif
