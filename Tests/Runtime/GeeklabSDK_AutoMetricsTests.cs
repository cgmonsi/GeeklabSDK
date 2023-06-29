using System.Collections;
using Kitrum.GeeklabSDK;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Assertions;
using UnityEngine.TestTools;
using Assert = NUnit.Framework.Assert;


public class GeeklabSDK_AutoMetricsTests {
    private static int secToStopTest = 5;
    private readonly WaitForSeconds waitForSeconds = new WaitForSeconds(0.1f);

    
    [UnityTest]
    public IEnumerator InitializePurchasing() {
        var elapsedTime = 0f;
        while ((PurchaseMetrics.Instance == null || PurchaseMetrics.Instance.IsUnityPurchaseReady == null)
               && elapsedTime < secToStopTest)
        {
            yield return waitForSeconds;
            elapsedTime += 0.1f;
        }
        
        
        // If we exceeded the timeout duration, fail the test.
        if (elapsedTime >= secToStopTest)
        {
            Assert.Fail("Test timed out.");
        }
    
        // Verify that the ShowAd method was called on the AdMetrics instance
        if (PurchaseMetrics.Instance.IsUnityPurchaseReady != null)
            Assert.IsTrue(PurchaseMetrics.Instance.IsUnityPurchaseReady);
    }
    
    
    [UnityTest]
    public IEnumerator InitializeAd()
    {
        var elapsedTime = 0f;
        while ((AdMetrics.Instance == null || AdMetrics.Instance.IsUnityAdsReady == null)
               && elapsedTime < secToStopTest)
        {
            yield return waitForSeconds;
            elapsedTime += 0.1f;
        }
        
        // If we exceeded the timeout duration, fail the test.
        if (elapsedTime >= secToStopTest)
        {
            Assert.Fail("Test timed out.");
        }
        
        // Verify that the ShowAd method was called on the AdMetrics instance
        if (AdMetrics.Instance.IsUnityAdsReady != null)
            Assert.IsTrue(AdMetrics.Instance.IsUnityAdsReady);
        
        yield return waitForSeconds;

    }
}