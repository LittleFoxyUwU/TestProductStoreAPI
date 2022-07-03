using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProductStoreAPI.Models;


namespace ProductStoreAPI;

public static class WebApiApp
{
    public const string AppName = "ProductStoreAPI";
    public const string SecretKey = "TOP_SECRET_KEY_VERY_LONG_ONE_SO_IT_WILL_WORK_NORMALLY";
    public static WebApplication WebApplication = null!;
    
    public static ref WebApplication AppInitialisator(string[] args)
    {
        var options = new WebApplicationOptions
        {
            Args = args,
            ApplicationName = AppName
        };

        var builder = WebApplication.CreateBuilder(options);

        var connectionStringProducts = builder.Configuration.GetConnectionString("Database") 
                                       ?? "Data Source=Database.db";

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddRazorPages();

        builder.Services.AddSqlite<AppDatabase>(connectionStringProducts);

        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(op =>
            {
                op.LoginPath = "/login";
                op.LogoutPath = "/logout";
            });

        builder.Services.AddAuthorization();

        builder.Services.AddSwaggerGen(c =>
        {  
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "ProductStoreAPI",
                Description = "Making the Products you love", 
                Version = "v1" 
            });
        });

        var app = builder.Build();
        
        if (app.Environment.IsDevelopment()) app.UseDeveloperExceptionPage();

        app.UseSwagger();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapRazorPages();
        
        app.UseSwaggerUI(c => {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProductStoreAPI API V1");
        });
        
        WebApplication = app;
        return ref WebApplication;
    }
}