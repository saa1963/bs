using bs;
using bs.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Server.IISIntegration;
using NLog;
using NLog.Web;
using Blazored.LocalStorage;

//NLog.ILogger logger = LogManager.GetCurrentClassLogger();
var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    builder.WebHost.UseNLog();
    builder.WebHost.UseUrls("http://*:6005");

    // Add services to the container.
    //builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
    //   .AddNegotiate();

    //builder.Services.AddAuthorization(options =>
    //{
    //// By default, all incoming requests will be authorized according to the default policy.
    //options.FallbackPolicy = options.DefaultPolicy;
    //    options.AddPolicy("New", policy =>
    //                      policy.RequireClaim("permission", "MarkNew"));
    //    options.AddPolicy("Put", policy =>
    //                    policy.RequireClaim("permission", "MarkPut"));
    //    options.AddPolicy("Withdrawal", policy =>
    //                    policy.RequireClaim("permission", "MarkWithdrawal"));
    //});
    //builder.Services.AddSingleton<ValidateAuthentication>();

    builder.Services.AddScoped<IClaimsTransformation, ClaimsTransformer>(claimFactory);

    builder.Services.AddRazorPages();
    builder.Services.AddServerSideBlazor();


    ClaimsTransformer claimFactory(IServiceProvider arg)
    {
        string[] p_new = builder.Configuration.GetSection("ValidUsers:new").AsEnumerable().
                Where(s => s.Value != null).Select(s => new string(s.Value)).ToArray();
        string[] p_put = builder.Configuration.GetSection("ValidUsers:put").AsEnumerable().
                Where(s => s.Value != null).Select(s => new string(s.Value)).ToArray();
        string[] p_withdrawal = builder.Configuration.GetSection("ValidUsers:withdrawal").AsEnumerable().
                Where(s => s.Value != null).Select(s => new string(s.Value)).ToArray();
        return new ClaimsTransformer(p_new, p_put, p_withdrawal);
    }

    builder.Services.AddBlazoredLocalStorage();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
    }


    app.UseStaticFiles();

    app.UseRouting();

    
    //app.UseAuthentication();
    //app.UseAuthorization();
    //app.UseMiddleware<ValidateAuthentication>();

    app.MapBlazorHub();
    app.MapFallbackToPage("/_Host");

    app.Run();

}
catch (Exception e)
{
    logger.Error(e, "Stopped program because of exception");
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}