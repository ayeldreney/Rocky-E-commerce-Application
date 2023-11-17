using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Rocky.BLL.Constants;
using Rocky.BLL.Helpers;
using Rocky.BLL.Middlewares;
using Rocky.BLL.Services;
using Rocky.DAL.Data;
using Rocky.DAL.Models;
using Rocky.Localizations;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

#region Adding Db
string? ConnectionString = builder.Configuration.GetConnectionString("Rockey");
builder.Services.AddDbContext<ApplicationDBcontext>(options => options.UseSqlServer(ConnectionString));
#endregion

#region Adding Identity Service

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;

    options.Lockout.MaxFailedAccessAttempts = AppSettings.UserSettings.MaxFailedAccessAttempts;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(AppSettings.UserSettings.LockoutTimeSpanInMinutes);

}).AddEntityFrameworkStores<ApplicationDBcontext>();

#region Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "default";
    options.DefaultChallengeScheme = "default";
})
.AddCookie(config =>
{
    config.Cookie.Name = AppSettings.UserSettings.TokenBearerCookieName;
})
.AddJwtBearer("default", options =>
{
    options.Events = new JwtBearerEvents()
    {
        OnMessageReceived = context =>
        {
            string? _token = "";
            context.Request.Cookies.TryGetValue(AppSettings.UserSettings.TokenBearerCookieName, out _token);
            context.Token = _token;
            return Task.CompletedTask;
        }
    };
    var jwtOptions = builder.Configuration.GetSection("JWT");
    var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtOptions.GetValue<string>("Key")!));

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        IssuerSigningKey = key,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
    };
});
#endregion

builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));

builder.Services.AddTransient<AuthService>();

#endregion

#region Localization
builder.Services.AddLocalization(options => options.ResourcesPath = Rocky.BLL.Constants.AppSettings.LocalizationResourcesPath);

builder.Services.AddMvc()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
        {
            var assemblyName = new AssemblyName(typeof(Phrase).GetTypeInfo().Assembly.FullName!);
            return factory.Create("DataAnnotations", assemblyName.Name!);
        };
    });
builder.Services.AddSingleton<Phrase>();

#endregion


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseMiddleware<CultureMiddleware>();

#region Declare Using Authorization and Authentication 

app.UseAuthentication();
app.UseAuthorization();

#endregion

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

