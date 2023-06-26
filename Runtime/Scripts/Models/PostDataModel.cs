using System;


namespace Kitrum.GeeklabSDK
{
    [Serializable]
    public class PostData
    {
        public string id;
        public string type;
        public string created;
        public object data;
    }
}