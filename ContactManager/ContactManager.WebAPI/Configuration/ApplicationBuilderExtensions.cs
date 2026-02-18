using Microsoft.AspNetCore.Builder;

namespace ContactManager.WebAPI.Configuration;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseCoreConfiguration(this WebApplication app)
    {
        app.UseExceptionHandler("/Home/Error");

        app.UseHsts();

        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseAuthorization();

        app.MapStaticAssets();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Contact}/{action=Index}/{id?}")
            .WithStaticAssets();

        return app;
    }
}
