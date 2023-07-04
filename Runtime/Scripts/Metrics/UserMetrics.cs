using System;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Kitrum.GeeklabSDK
{
    public class UserMetrics : MonoBehaviour
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
        
        
        public static async Task<bool> SendMetrics(string postData = null)
        {
            if (!SDKSettingsModel.Instance.SendStatistics) 
                return false;

            var taskCompletionSource = new TaskCompletionSource<bool>();
            
            WebRequestManager.Instance.SendUserMetricsRequest(postData,
                (response) =>
                {
                    if (SDKSettingsModel.Instance.ShowDebugLog)
                        Debug.Log(
                            $"{SDKSettingsModel.GetColorPrefixLog()} {response}");
                    taskCompletionSource.SetResult(true);
                },
                (error) =>
                {
                    Debug.LogError(error);
                    taskCompletionSource.SetResult(false);
                }
            );
            
            return await taskCompletionSource.Task;
        }
    }
}