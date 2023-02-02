using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using VerdonFileManager.Services;
using VerdonFileManager;
using VerdonFileManager.Areas.Identity;
using VerdonFileManager.Data;
using System.Security.Permissions;
using Microsoft.AspNetCore.Http.Features;
using VerdonFileManager.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddIdentity<AppUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddDefaultTokenProviders()
    .AddDefaultUI()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<AppUser>>();
builder.Services.AddSingleton<MailSettings>();
builder.Services.AddSingleton<MailService>();
builder.Services.AddTransient<UserService>();
builder.Services.AddTransient<UploadService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});
builder.Services.Configure<FormOptions>(o =>
{
    o.ValueLengthLimit = 209715200;
    o.MultipartBodyLengthLimit = 209715200;
    o.MemoryBufferThreshold = 209715200;
});

// configure static file permission
FileIOPermission permission = new FileIOPermission(FileIOPermissionAccess.AllAccess, Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot", "StaticFiles"));
permission.AddPathList(FileIOPermissionAccess.AllAccess, Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot", "UserImages"));
permission.Demand();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseCors("CorsPolicy");
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot", "StaticFiles")),
    RequestPath = new PathString("/wwwroot/StaticFiles")
});

app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot", "UserImages")),
    RequestPath = new PathString("/wwwroot/UserImages")
});
app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();
