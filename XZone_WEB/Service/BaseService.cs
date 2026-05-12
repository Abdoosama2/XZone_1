using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using XZone_WEB.Models;
using XZone_WEB.Service.IService;
using XZoneUtility;
    
namespace XZone_WEB.Service
{
    public class BaseService : IBaseService
    {
        
        public ApiRequest _request { get; set; }
        public IHttpClientFactory _httpClientFactory { get; set; }

        public BaseService(IHttpClientFactory httpClientFactory)
        {
            this._httpClientFactory = httpClientFactory;
            this._request = new ApiRequest();
            
        }
        public async Task<T> SendAsync<T>(ApiRequest request)
        {
            try
            {
                // 1) Create HttpClient from factory
                var client = _httpClientFactory.CreateClient("XZone");

                // 2) Create HTTP request message
                HttpRequestMessage message = new HttpRequestMessage();

                // 3) Accept JSON response
                message.Headers.Add("Accept", "application/json");

                // 4) Set request URL
                message.RequestUri = new Uri(request.URL);

                // 5) Add request body if exists
                if (request.Data is MultipartFormDataContent multiPartContent)
                {
                    // If sending file/form-data
                    message.Content = multiPartContent;
                }
                else if (request.Data != null)
                {
                    // If sending normal JSON body
                    message.Content = new StringContent(
                        JsonConvert.SerializeObject(request.Data),
                        Encoding.UTF8,
                        "application/json");
                }

                // 6) Set HTTP method (GET, POST, PUT, DELETE)
                switch (request.ApiType)
                {
                    case SD.ApiType.Post:
                        message.Method = HttpMethod.Post;
                        break;

                    case SD.ApiType.Put:
                        message.Method = HttpMethod.Put;
                        break;

                    case SD.ApiType.Delete:
                        message.Method = HttpMethod.Delete;
                        break;

                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }

                // 7) Add Bearer Token to REQUEST (correct way)
                if (!string.IsNullOrEmpty(request.Token))
                {
                    message.Headers.Authorization =
                        new AuthenticationHeaderValue("Bearer", request.Token);
                }

              

                // 8) Send request to API
                HttpResponseMessage apiResponse = await client.SendAsync(message);

                // 9) Read response content as string
                var apiContent = await apiResponse.Content.ReadAsStringAsync();

                try
                {
                    // 10) Deserialize response into T
                    var responseObject = JsonConvert.DeserializeObject<T>(apiContent);

                    // 11) Handle known error status codes manually
                    if (responseObject is ApiResponse response &&
                        (apiResponse.StatusCode == System.Net.HttpStatusCode.BadRequest ||
                         apiResponse.StatusCode == System.Net.HttpStatusCode.NotFound))
                    {
                        response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                        response.IsSuccess = false;
                        return responseObject;
                    }

                    // 12) Return successful response
                    return responseObject;
                }
                catch (Exception ex)
                {
                    // 13) If deserialization fails → return error response
                    var dto = new ApiResponse()
                    {
                        ErrorMessages = new List<string> { ex.Message },
                        IsSuccess = false
                    };

                    var result = JsonConvert.SerializeObject(dto);
                    var responseObject = JsonConvert.DeserializeObject<T>(result);

                    return responseObject;
                }
            }
            catch (Exception ex)
            {
                // 14) Handle unexpected exceptions (network, etc.)
                var dto = new ApiResponse()
                {
                    ErrorMessages = new List<string> { ex.Message },
                    IsSuccess = false
                };

                var result = JsonConvert.SerializeObject(dto);
                var responseObject = JsonConvert.DeserializeObject<T>(result);

                return responseObject;
            }
        }

        

    }
    }

