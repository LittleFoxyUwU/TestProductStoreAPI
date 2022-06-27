using Microsoft.EntityFrameworkCore;
using ProductStoreAPI;
using ProductStoreAPI.Models;
using ProductStoreAPI.Routes;

var app = WebApiApp.AppInitialisator(args);

ProductRoutes.SetRoutes(ref app);
UserRoutes.SetRoutes(ref app);

app.MapGet("/", (HttpRequest request, HttpContext context) =>
{
    var path = Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(request);
    context.Response.ContentType = "text/html; charset=utf-8";
    var responseBody = $"<html><body><h1>Welcome to Foxy's Products Store!</h1><a href='{path + "swagger"}'>Please refer to Swagger</a></body></html>";
    return responseBody;
});

app.Run();

