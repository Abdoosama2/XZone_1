using XZone_WEB.Service;
using XZone_WEB.Service.IService;

namespace XZone_WEB
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Register core services first
            builder.Services.AddHttpClient();
            builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
            builder.Services.AddAutoMapper(typeof(MappingConfig));
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Register application services
            builder.Services.AddScoped<IGameService, GameService>();

            builder.Services.AddScoped<ICategoryService, CategoryService>();

            builder.Services.AddScoped<IDeviceService, DeviceService>();

            builder.Services.AddScoped<IUserService, UserService>();
            // Register your GameService



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
