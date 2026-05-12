using Newtonsoft.Json;
using System.Net.Http.Headers;
using XZone_WEB.Models.DTO.GameDTOs;
using XZone_WEB.Service.IService;
using XZoneUtility;

namespace XZone_WEB.Service
{
    public class GameService : BaseService, IGameService
    {

        private readonly IHttpClientFactory _factory;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private string _GameURL;

        public GameService(IHttpClientFactory httpClientFactory, IConfiguration config, IWebHostEnvironment webHostEnvironment) : base(httpClientFactory)
        {
            _factory = httpClientFactory;
            _GameURL = config.GetValue<string>("ServiceUrls:XZoneAPI");
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<T> CreateAsync<T>(GameCreateDTO GameDto)
        {
            string imageUrl = null; // To store the generated image URL

            if (GameDto.ImageFile != null)
            {
                // Validate file type
                string[] allowedTypes = { "image/jpeg", "image/jpg", "image/png" };
                if (!allowedTypes.Contains(GameDto.ImageFile.ContentType))
                {
                    throw new InvalidOperationException("Only JPEG and PNG images are allowed.");
                }

                // Validate file size (5MB max)
                if (GameDto.ImageFile.Length > 5 * 1024 * 1024)
                {
                    throw new InvalidOperationException("File size cannot exceed 5MB.");
                }

                // Generate unique filename
                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(GameDto.ImageFile.FileName)}";
                string relativePath = Path.Combine("images", "games", fileName);
                string absolutePath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath);

                // Ensure directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(absolutePath));

                // Save the file
                using (var stream = new FileStream(absolutePath, FileMode.Create))
                {
                    await GameDto.ImageFile.CopyToAsync(stream);
                }

                // Set the URL for the API (using forward slashes for URLs)
                imageUrl = "/" + relativePath.Replace("\\", "/");
                GameDto.ImageURL = imageUrl; // Optionally update the DTO if needed
            }

            // Create a new DTO to send to the API, containing the ImageURL
            var apiCreateDto = new
            {
                Name = GameDto.Name,
                CategoryId = GameDto.CategoryId,
                Description = GameDto.Description,
                ImageURL = imageUrl, // Send the generated URL
                SelectedDevices = GameDto.SelectedDevices
            };

            return await SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.Post,
                Data = apiCreateDto, // Send the new DTO as JSON
                URL = _GameURL + "/api/Game/"
            });
        }

        public Task<T> DeleteAsync<T>(int id, string token)
        {
            return SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.Delete,
              
                URL = _GameURL + "/api/Game/"+id,
                Token = token,

            });
        }

        public Task<T> GetAllAsync<T>()
        {
            return SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.Get,
              
                URL = _GameURL + "/api/Game/",
               // Token = token,

            });
        }

        public Task<T> GetAsync<T>(int id, string token)
        {
            return SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.Get,
               
                URL = _GameURL + "/api/Game/"+id,
                Token = token,

            });
        }

        public Task<T> UpdateAsync<T>(GameUpdateDTO GameDto, string token)
        {
            return SendAsync<T>(new ApiRequest
            {
                ApiType = SD.ApiType.Put,
                Data = GameDto,
                URL = _GameURL + "/api/Game/"+GameDto.Id,
                Token = token,

            });
        }
    }
}
