public static class ApiEndpointsModel
{
    private const string API_ENDPOINT = "https://your-api-endpoint.com/";

    public const string CHECK_DATA_COLLECTION_STATUS = API_ENDPOINT + "CheckCollection";
    public const string GET_TOKEN = API_ENDPOINT + "GetToken";
    public const string SEND_TOKEN = API_ENDPOINT + "SetToken";
    
    public const string SEND_DEVICE_INFO = API_ENDPOINT + "sendDeviceInfo";
    public const string SEND_AD_METRICS = API_ENDPOINT + "sendAdMetrics";
    public const string SEND_ENGAGEMENT_METRICS = API_ENDPOINT + "sendEngagementMetrics";
    public const string SEND_PURCHASE_METRICS = API_ENDPOINT + "sendPurchaseMetrics";
}