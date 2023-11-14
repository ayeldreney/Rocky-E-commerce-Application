using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Rocky.BLL.Helpers;
using Rocky.BLL.Middlewares;
using Rocky.BLL.Services;
using Rocky.DAL.Data;
using Rocky.DAL.Models;
using Rocky.Localizations;
using System.Reflection;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

#region Adding Db
string? ConnectionString = builder.Configuration.GetConnectionString("Rockey");
builder.Services.AddDbContext<ApplicationDBcontext>(options => options.UseSqlServer(ConnectionString));
#endregion

#region Localization
builder.Services.AddLocalization(options => options.ResourcesPath = Rocky.BLL.Constants.General.LocalizationResourcesPath);

builder.Services.AddMvc()
	.AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
	.AddDataAnnotationsLocalization(options => {
		options.DataAnnotationLocalizerProvider = (type, factory) =>
		{
			var assemblyName = new AssemblyName(typeof(Phrase).GetTypeInfo().Assembly.FullName!);
			return factory.Create("DataAnnotations", assemblyName.Name!);
		};
	});
builder.Services.AddSingleton<Phrase>();

#endregion

#region Adding Identity Service
builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = "Default";
	options.DefaultChallengeScheme = "Default"; // what action to do when unauthorized
}).AddJwtBearer("Default", options =>
{
	var jwtOptions = builder.Configuration.GetSection("JWT");
	string keyInStr = jwtOptions.GetValue<string>("Key")!;
	byte[] keyInBytes = Encoding.ASCII.GetBytes(keyInStr);
	var key = new SymmetricSecurityKey(keyInBytes);

	options.TokenValidationParameters = new TokenValidationParameters
	{
		IssuerSigningKey = key,
		ValidIssuer = jwtOptions.GetValue<string>("Issuer"),
		ValidAudience = jwtOptions.GetValue<string>("Audience"),
		ValidateIssuer = true,
		ValidateAudience = true,
	};
});
builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));

builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDBcontext>();

builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("AdminPolicy", p => p.RequireClaim(ClaimTypes.Role, "Admin"));
});

builder.Services.AddTransient<AuthService>();

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

