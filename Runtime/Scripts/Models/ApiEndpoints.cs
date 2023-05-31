public static class ApiEndpoints
{
    private const string API_ENDPOINT = "https://your-api-endpoint.com/";

    public const string CHECK_DATA_COLLECTION_STATUS = API_ENDPOINT + "CheckCollection";
    public const string GET_TOKEN = API_ENDPOINT + "GetToken";
    public const string SEND_TOKEN = API_ENDPOINT + "SetToken";
    
    public const string SEND_DEVICE_INFO = API_ENDPOINT + "sendDeviceInfo";
    public const string SEND_AD_METRICS = API_ENDPOINT + "sendAdMetrics";
    public const string SEND_ENGAGEMENT_METRICS = API_ENDPOINT + "sendEngagementMetrics";
    public const string SEND_PURCHASE_METRICS = API_ENDPOINT + "sendPurchaseMetrics";
    
    
    /// ----Projects---- ///
    // Get all projects
    public const string PROJECTS_LIST = "https://geeklab.app/APIV1/v1/list/{geeklabToken}"; 
    // Filter projects by genre and subgenre
    public const string PROJECTS_FILTER_GENRE_SUBGENRE = "https://geeklab.app/APIV1/v1/list/{geeklabToken}?genre={genreCSV}&subgenre={subgenreCSV}";
    
    /// ----Campaigns---- ///
    // Get all campaigns
    public const string CAMPAIGNS_LIST = "https://geeklab.app/APIV1/v1/list/c/{geeklabToken}";
    // Filter campaigns by apps
    public const string CAMPAIGNS_FILTER_APP = "https://geeklab.app/APIV1/v1/list/c/{geeklabToken}?app={appToken}";

    /// ----Variants---- ///
    // Get all variants
    public const string VARIANTS_LIST = "https://geeklab.app/APIV1/v1/list/v/{geeklabToken}";
    // Filter variants by campaigns
    public const string VARIANTS_FILTER_CAMPAIGN = "https://geeklab.app/APIV1/v1/list/v/{geeklabToken}?campaign={campaignToken}";
    
    /// ----Results---- ///
    // Get results by app
    public const string RESULTS_APP = "https://geeklab.app/APIV1/v1/retrieve/{geeklabToken}&app=appId"; 
    // Get results by campaign
    public const string RESULTS_CAMPAIGN = "https://geeklab.app/APIV1/v1/retrieve/{geeklabToken}&campaign=campaigntoken";
    // Get results by variant
    public const string RESULTS_VARIANT = "https://geeklab.app/APIV1/v1/retrieve/{geeklabToken}&variant=varianttoken"; 
}