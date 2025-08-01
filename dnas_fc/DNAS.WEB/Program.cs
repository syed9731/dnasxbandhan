using DNAS.Application;
using DNAS.Application.Common.Filter;
using DNAS.Application.Middleware;
using DNAS.Persistence;
using DNAS.Shared;

using DNTCaptcha.Core;

using Microsoft.AspNetCore.Authentication.Cookies;

using System.Threading.RateLimiting;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
}).AddRazorRuntimeCompilation()
    .AddCookieTempDataProvider(options =>
    {
        options.Cookie.Name = ".AspNetCore.Mvc.CookieTempDataProvider";
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Secure only
        options.Cookie.HttpOnly = true; // Prevent client-side script access
    });

builder.Services.AddDNTCaptcha(options =>
{
    // options.UseSessionStorageProvider()                // -> It doesn't rely on the server or client's times. Also it's the safest one.
    // options.UseMemoryCacheStorageProvider()            // -> It relies on the server's times. It's safer than the CookieStorageProvider.
    options.UseCookieStorageProvider(SameSiteMode.Strict) // -> It relies on the server and client's times. It's ideal for scalability, because it doesn't save anything in the server's memory.
    // options.UseDistributedCacheStorageProvider()       // --> It's ideal for scalability using `services.AddStackExchangeRedisCache()` for instance.
    // options.UseDistributedSerializationProvider()

    // Don't set this line (remove it) to use the installed system's fonts (FontName = "Tahoma").
    // Or if you want to use a custom font, make sure that font is present in the wwwroot/fonts folder and also use a good and complete font!
    //.UseCustomFont(Path.Combine(_env.WebRootPath, "fonts", "IRANSans(FaNum)_Bold.ttf")) // This is optional.
    .AbsoluteExpiration(minutes: 7)
    .RateLimiterPermitLimit(10) // for .NET 7x+, Also you need to call app.UseRateLimiter() after calling app.UseRouting().
    .ShowThousandsSeparators(false)
    .WithNoise(0.015f, 0.015f, 1, 0.0f)
    .WithEncryptionKey("This is my secure key!")
    .WithNonceKey("NETESCAPADES_NONCE")
    .InputNames(// This is optional. Change it if you don't like the default names.
        new DNTCaptchaComponent
        {
            CaptchaHiddenInputName = "DNTCaptchaText",
            CaptchaHiddenTokenName = "DNTCaptchaToken",
            CaptchaInputName = "DNTCaptchaInputText"
        })
    .Identifier("dntCaptcha");// This is optional. Change it if you don't like its default name.




});



builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => false;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});
builder.Services.AddRateLimiter(option =>
{
    option.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    option.AddPolicy("fixed", HttpContext =>
    RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: HttpContext.Connection.RemoteIpAddress?.ToString(),
        factory: _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 3,
            Window = TimeSpan.FromSeconds(10)
        }));
});

 
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment() 
        ? CookieSecurePolicy.None 
        : CookieSecurePolicy.Always; // Send only over HTTPS in production
    options.Cookie.SameSite = SameSiteMode.Strict; // Prevent cross-site usage
});
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment() 
        ? CookieSecurePolicy.None 
        : CookieSecurePolicy.Always;
});


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
.AddCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
    options.Cookie.MaxAge = options.ExpireTimeSpan; // optional
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment() 
        ? CookieSecurePolicy.None 
        : CookieSecurePolicy.Always;
    options.LoginPath = new PathString("/Login");
    options.AccessDeniedPath = new PathString("/Login");
});
builder.AddLogApplication(builder.Configuration);
builder.Services
    .AddApplication()
    .AddPersistence()
    .AddShared();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHsts();
app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseStaticFiles();
app.UseCookiePolicy();
app.UseRouting();
app.UseMiddleware<RateLimitMiddleware>();
app.UseMiddleware<BlockBurpSuiteMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.UseMiddleware<OtpVaptMiddleware>();

app.UseXfo(o => o.Deny());//for X-Frame-Option vapt
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

await app.RunAsync();
