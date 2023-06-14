using NUnit.Framework;

public class GeeklabSDKTests {
    [Test]
    public void ToggleMetricsCollection_WhenCalled_ChangesCollectServerData() {
        // // Arrange
        // var initialCollectServerDataState = SDKInfoModel.CollectServerData;
        // var newCollectServerDataState = !initialCollectServerDataState;
        //
        // // Act
        // GeeklabSDK.ToggleMetricsCollection(newCollectServerDataState);
        //
        // // Assert
        // Assert.AreEqual(newCollectServerDataState, SDKInfoModel.CollectServerData);
    }
    
    [Test]
    public void InitializePurchasing_WhenCalled_CallsPurchaseMetricsInitializePurchasing() {
       
    }

    [Test]
    public void ShowAd_WhenCalled_CallsAdMetricsShowAd() {
       
    }

    [Test]
    public void SendDeviceInformation_WhenCalled_CallsDeviceInfoHandlerSendDeviceInfo() {
     
    }
}