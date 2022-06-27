namespace ProductStoreAPI.Routes;

public static class UserRoutes
{
    /// <summary>
    /// Sets all user-based routes to the given app
    /// </summary>
    public static void SetRoutes(ref WebApplication app)
    {
        SetGetRoutes(ref app);
       // SetPutRoutes(ref app);
       // SetPostRoutes(ref app);
       // SetDeleteRoutes(ref app);
    }

    private static void SetGetRoutes(ref WebApplication app)
    {
        app.MapGet("/login", async (HttpContext context) =>
        {
            context.Response.ContentType = "text/html; charset=utf-8";
            await context.Response.SendFileAsync("wwwroot/login.html");
        });
    }
}