using UnityEngine;

public class ClipboardHandler : MonoBehaviour
{
#if UNITY_EDITOR
    public static void WriteToClipboard(string str)
    {
        var te = new TextEditor {
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
    
#elif UNITY_IOS
    [DllImport("__Internal")]
    private static extern void _WriteToClipboard(string str);

    [DllImport("__Internal")]
    private static extern string _ReadFromClipboard();

    public void WriteToClipboard(string str)
    {
        _WriteToClipboard(str);
    }

    public string ReadClipboard()
    {
        return _ReadFromClipboard();
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
#endif
}