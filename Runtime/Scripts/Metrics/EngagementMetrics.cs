using UnityEngine;
using System.Collections.Generic;

public class EngagementMetrics : MonoBehaviour
{
    private static int daysLoggedIn;
    private static float sessionTime;
    private static int levelPassed;

    private void Start()
    {
        // Initialize metrics
        daysLoggedIn = 0;
        sessionTime = 0.0f;
        levelPassed = 0;
    }

    // Update is called once per frame
    private void Update()
    {
        // Update session time
        sessionTime += Time.deltaTime;
    }

    public static void IncrementDaysLoggedIn()
    {
        daysLoggedIn++;
    }

    public static void IncrementLevelPassed()
    {
        levelPassed++;
    }
    
    public static void UpdateMetrics()
    {
    }

    public static void SendMetrics()
    {
        if (!SDKSettingsModel.Instance.SendStatistics) return;

        var postData = new Dictionary<string, string>
        {
            { "daysLoggedIn", daysLoggedIn.ToString() },
            { "sessionTime", sessionTime.ToString() },
            { "levelPassed", levelPassed.ToString() }
        };

        var json = JsonUtility.ToJson(postData);
        WebRequestManager.Instance.SendEngagementMetricsRequest(json, Debug.Log, Debug.LogError);
    }
}
