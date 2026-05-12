


using static XZoneUtility.SD;

namespace XZone_WEB
{
    public class ApiRequest
    {
        public ApiType ApiType { get; set; }
        public string ? URL { get; set; }

        public Object? Data { get; set; }

        public string? Token { get; set; }
    }
}
