package com.kitrum.plugin;

import android.content.ClipData;
import android.content.ClipboardManager;
import android.content.Context;
import com.unity3d.player.UnityPlayer;

public class ClipboardHelper {
    public static void SetText(String text) {
        Context context = UnityPlayer.currentActivity;
        ClipboardManager clipboard = (ClipboardManager) context.getSystemService(Context.CLIPBOARD_SERVICE);
        ClipData clip = ClipData.newPlainText("label", text);
        clipboard.setPrimaryClip(clip);
    }

    public static String GetText() {
        Context context = UnityPlayer.currentActivity;
        ClipboardManager clipboard = (ClipboardManager) context.getSystemService(Context.CLIPBOARD_SERVICE);
        ClipData clip = clipboard.getPrimaryClip();
        if (clip != null && clip.getItemCount() > 0) {
            return clip.getItemAt(0).coerceToText(context).toString();
        }
        return null;
    }
}
