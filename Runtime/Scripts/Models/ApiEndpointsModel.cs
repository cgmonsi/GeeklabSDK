namespace Kitrum.GeeklabSDK
{
    public static class ApiEndpointsModel
    {
        private const string API_ENDPOINT = "https://analytics.geeklab.app/";
        
        public const string TEST_TOKEN = "c86fb1b2-e3bf-4e91-8299-e3d203a8d36d";

        public const string CHECK_DATA_COLLECTION_STATUS = API_ENDPOINT + "CheckCollection";
        public const string GET_TOKEN = API_ENDPOINT + "GetToken";
        public const string SEND_TOKEN = API_ENDPOINT + "SetToken";

        public const string VERIFY_API_KEY = API_ENDPOINT + "auth";
        
        public const string WEBHOOK = API_ENDPOINT + "webhook";
    }
}