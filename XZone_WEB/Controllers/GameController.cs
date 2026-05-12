using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Collections.Generic;
using XZone_WEB.Models;
using XZone_WEB.Models.DTO.CategoryDTo;
using XZone_WEB.Models.DTO.DeviceDTOs;
using XZone_WEB.Models.DTO.GameDTOs;
using XZone_WEB.Service.IService;
using XZoneUtility;

namespace XZone_WEB.Controllers
{
    public class GameController : Controller
    {
        //private readonly IMapper mapper;
        private readonly IGameService gameService;
        private readonly ICategoryService categoryService;
        private readonly IDeviceService deviceService;
        private readonly ICategoryService categoryService1;

        public GameController(IGameService gameService, ICategoryService categoryService,IDeviceService deviceService )
        {
           // this.mapper = mapper;
            this.gameService = gameService;
            this.categoryService = categoryService;
            this.deviceService = deviceService;
        }

        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> Create()
        {
            var categoryResponse = await categoryService.GetAllAsync<ApiResponse>();
            List<CategoryDTO> categories = new List<CategoryDTO>();
            if (categoryResponse != null && categoryResponse.IsSuccess)
            {
                categories = JsonConvert.DeserializeObject<List<CategoryDTO>>(Convert.ToString(categoryResponse.Result));
            }

            var deviceResponse = await deviceService.GetAllAsync<ApiResponse>();
            List<DeviceDTO> devices = new List<DeviceDTO>();
            if (deviceResponse != null && deviceResponse.IsSuccess)
            {
                devices = JsonConvert.DeserializeObject<List<DeviceDTO>>(Convert.ToString(deviceResponse.Result));
            }

            var gameCreateDTO = new GameCreateDTO
            {
                Categories = categories.Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                }),
                devices = devices.Select(d => new SelectListItem
                {
                    Text = d.Name,
                    Value = d.Id.ToString()
                })
            };

            return View(gameCreateDTO);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GameCreateDTO gameCreateDTO)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Call the GameService to handle image upload and API call
                    var response = await gameService.CreateAsync<ApiResponse>(gameCreateDTO);
                    if (response != null && response.IsSuccess)
                    {
                        TempData["success"] = "Game created successfully";
                        return RedirectToAction(nameof(Index));
                    }
                    else if (response?.ErrorMessages?.Any() == true)
                    {
                        ModelState.AddModelError("", response.ErrorMessages.First());
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to create game. Please try again.");
                    }
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("ImageFile", ex.Message);
                }
            }

            // If ModelState is not valid or an exception occurred, reload the view with errors
            await ReloadCreateView(gameCreateDTO);
            return View(gameCreateDTO); // Ensure you return the model to retain dropdown values
        }

        private async Task<GameCreateDTO> ReloadCreateView(GameCreateDTO model)
        {
            var categoryResponse = await categoryService.GetAllAsync<ApiResponse>();
            List<CategoryDTO> categories = new List<CategoryDTO>();
            if (categoryResponse != null && categoryResponse.IsSuccess)
            {
                categories = JsonConvert.DeserializeObject<List<CategoryDTO>>(Convert.ToString(categoryResponse.Result));
            }

            var deviceResponse = await deviceService.GetAllAsync<ApiResponse>();
            List<DeviceDTO> devices = new List<DeviceDTO>();
            if (deviceResponse != null && deviceResponse.IsSuccess)
            {
                devices = JsonConvert.DeserializeObject<List<DeviceDTO>>(Convert.ToString(deviceResponse.Result));
            }

            model.Categories = categories.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            });
            model.devices = devices.Select(d => new SelectListItem
            {
                Text = d.Name,
                Value = d.Id.ToString()
            });

            return model;
        }


       // [HttpDelete]

        //public Task<IActionResult> Delete(int id)
        //{


        //}
    }
}
