// DeviceGeneration.java
package com.kitrum.plugin;

import android.os.Build;

public class DeviceGeneration {
    public static String GetDeviceGeneration() {
        return Build.MODEL;
    }
}