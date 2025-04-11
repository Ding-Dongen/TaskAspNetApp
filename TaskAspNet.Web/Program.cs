using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskAspNet.Business.Interfaces;
using TaskAspNet.Business.Services;
using TaskAspNet.Data.Context;
using TaskAspNet.Data.Interfaces;
using TaskAspNet.Data.Models;
using TaskAspNet.Data.Repositories;
using TaskAspNet.Web.Hubs;
using TaskAspNet.Web.Interfaces;
using TaskAspNet.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
    options.MaximumReceiveMessageSize = 102400; 
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
    options.HandshakeTimeout = TimeSpan.FromSeconds(15);
});
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        policyBuilder
            .SetIsOriginAllowed(_ => true)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});

builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection")));

builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<UserDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Auth/LogIn";
    options.AccessDeniedPath = "/Auth/AccessDenied";
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
    .AddCookie()
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
    });

builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<INotificationHubService, NotificationHubService>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IProjectStatusRepository, ProjectStatusRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationTypeRepository, NotificationTypeRepository>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<ILoggerService, LoggerService>();
builder.Services.AddScoped<IGoogleAuthHandler, GoogleAuthHandler>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<RoleService>();

var app = builder.Build();

app.UseHsts();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=LogIn}/{id?}");

app.MapHub<NotificationHub>("/notificationHub", options =>
{
    options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets;
    options.CloseOnAuthenticationExpiration = true;
});

// Defer role and user seeding to a background task
_ = Task.Run(async () =>
{
    using var scope = app.Services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

    string[] roleNames = { "SuperAdmin", "Admin", "User" };

    foreach (var role in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    string superAdminEmail = "superadmin@example.com";
    string superAdminPassword = "SuperAdmin123!";

    var superAdmin = await userManager.FindByEmailAsync(superAdminEmail);
    if (superAdmin == null)
    {
        var newSuperAdmin = new AppUser
        {
            UserName = superAdminEmail,
            Email = superAdminEmail,
            EmailConfirmed = true,
            FirstName = "Super",
            LastName = "Admin"
        };

        var result = await userManager.CreateAsync(newSuperAdmin, superAdminPassword);
        if (result.Succeeded)
        {
            if (!await userManager.IsInRoleAsync(newSuperAdmin, "SuperAdmin"))
            {
                await userManager.AddToRoleAsync(newSuperAdmin, "SuperAdmin");
            }
        }
    }
    else
    {
        if (!await userManager.IsInRoleAsync(superAdmin, "SuperAdmin"))
        {
            await userManager.AddToRoleAsync(superAdmin, "SuperAdmin");
        }
    }
});

app.Run();


//using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using TaskAspNet.Business.Interfaces;
//using TaskAspNet.Business.Services;
//using TaskAspNet.Data.Context;
//using TaskAspNet.Data.Models;
//using TaskAspNet.Data.Interfaces;
//using TaskAspNet.Data.Repositories;
//using TaskAspNet.Web.Hubs;
//using TaskAspNet.Web.Interfaces;
//using TaskAspNet.Web.Services;

//var builder = WebApplication.CreateBuilder(args);

//// ------------------
//// Services
//// ------------------
//builder.Services.AddControllersWithViews()
//    .AddJsonOptions(opt =>
//    {
//        opt.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
//    });

//// Register application services
//builder.Services.AddScoped<IProjectService, ProjectService>();
//builder.Services.AddScoped<IMemberService, MemberService>();
//builder.Services.AddScoped<INotificationService, NotificationService>();
//builder.Services.AddScoped<INotificationHubService, NotificationHubService>();

//// Repositories
//builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
//builder.Services.AddScoped<IMemberRepository, MemberRepository>();
//builder.Services.AddScoped<IClientRepository, ClientRepository>();
//builder.Services.AddScoped<IProjectStatusRepository, ProjectStatusRepository>();
//builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
//builder.Services.AddScoped<INotificationTypeRepository, NotificationTypeRepository>();

//// Others
//builder.Services.AddScoped<IApplicationService, ApplicationService>();
//builder.Services.AddScoped<IFileService, FileService>();
//builder.Services.AddScoped<ILoggerService, LoggerService>();
//builder.Services.AddScoped<IGoogleAuthHandler, GoogleAuthHandler>();
//builder.Services.AddScoped<UserService>();
//builder.Services.AddScoped<RoleService>();

//// SignalR
//builder.Services.AddSignalR(options =>
//{
//    options.EnableDetailedErrors = true;
//    options.MaximumReceiveMessageSize = 102_400;
//    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
//    options.HandshakeTimeout = TimeSpan.FromSeconds(15);
//});

//// CORS
//builder.Services.AddCors(options =>
//{
//    options.AddDefaultPolicy(policy =>
//    {
//        policy.SetIsOriginAllowed(_ => true)
//              .AllowAnyHeader()
//              .AllowAnyMethod()
//              .AllowCredentials();
//    });
//});

//// Logging
//builder.Services.AddLogging(log =>
//{
//    log.AddConsole();
//    log.AddDebug();
//});

//// ------------------
//// DbContexts
//// ------------------

//// Identity DbContext
//builder.Services.AddDbContext<UserDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection")));

//// App Logic DbContext
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//// ------------------
//// Identity + Auth
//// ------------------
//builder.Services.AddIdentity<AppUser, IdentityRole>()
//    .AddEntityFrameworkStores<UserDbContext>()
//    .AddDefaultTokenProviders();

//builder.Services.ConfigureApplicationCookie(options =>
//{
//    options.LoginPath = "/Auth/LogIn";
//    options.AccessDeniedPath = "/Auth/AccessDenied";
//    options.Cookie.SameSite = SameSiteMode.Lax;
//    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
//    options.Cookie.HttpOnly = true;
//    options.ExpireTimeSpan = TimeSpan.FromDays(30);
//});

//// Google Authentication
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//})
//    .AddCookie()
//    .AddGoogle(googleOptions =>
//    {
//        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
//        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
//    });

//builder.WebHost.UseWebRoot("wwwroot");

//var app = builder.Build();

//// ------------------
//// Middleware
//// ------------------
//app.UseHsts();
//app.UseHttpsRedirection();
//app.UseStaticFiles();
//app.UseRouting();
//app.UseCors();
//app.UseAuthentication();
//app.UseAuthorization();

//// ------------------
//// Routing
//// ------------------
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Auth}/{action=LogIn}/{id?}");

//app.MapHub<NotificationHub>("/notificationHub");

//// ------------------
//// Role + User Seeding
//// ------------------
//using (var scope = app.Services.CreateScope())
//{
//    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
//    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

//    string[] roles = { "SuperAdmin", "Admin", "User" };
//    foreach (var role in roles)
//    {
//        if (!await roleManager.RoleExistsAsync(role))
//        {
//            await roleManager.CreateAsync(new IdentityRole(role));
//        }
//    }

//    var email = "superadmin@example.com";
//    var password = "SuperAdmin123!";
//    var user = await userManager.FindByEmailAsync(email);

//    if (user == null)
//    {
//        var newUser = new AppUser
//        {
//            UserName = email,
//            Email = email,
//            EmailConfirmed = true,
//            FirstName = "Super",
//            LastName = "Admin"
//        };

//        var result = await userManager.CreateAsync(newUser, password);
//        if (result.Succeeded)
//        {
//            await userManager.AddToRolesAsync(newUser, roles);
//        }
//    }
//    else
//    {
//        foreach (var role in roles)
//        {
//            if (!await userManager.IsInRoleAsync(user, role))
//            {
//                await userManager.AddToRoleAsync(user, role);
//            }
//        }
//    }
//}

//// ------------------
//// Run the app
//// ------------------
//app.Run();



//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using TaskAspNet.Business.Interfaces;
//using TaskAspNet.Business.Services;
//using TaskAspNet.Data.Context;
//using TaskAspNet.Data.Interfaces;
//using TaskAspNet.Data.Repositories;
//using TaskAspNet.Data.Models;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using TaskAspNet.Web.Interfaces;
//using TaskAspNet.Web.Services;
//using TaskAspNet.Web.Hubs;

//var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddControllersWithViews();

//builder.Services.AddScoped<IProjectService, ProjectService>();
//builder.Services.AddScoped<IMemberService, MemberService>();

//builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
//builder.Services.AddScoped<IMemberRepository, MemberRepository>();
//builder.Services.AddScoped<IClientRepository, ClientRepository>();
//builder.Services.AddScoped<IProjectStatusRepository, ProjectStatusRepository>();
//builder.Services.AddScoped<IApplicationService, ApplicationService>();
//builder.Services.AddScoped<IFileService, FileService>();
//builder.Services.AddScoped<ILoggerService, LoggerService>();


//builder.Services.AddScoped<IGoogleAuthHandler, GoogleAuthHandler>();



//builder.Services.AddScoped<UserService>();
//builder.Services.AddScoped<RoleService>();


//builder.Services.AddLogging(logging =>
//{
//    logging.AddConsole(); 
//    logging.AddDebug();   
//});

//builder.Services.AddDbContext<UserDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection")));

//builder.Services.AddIdentity<AppUser, IdentityRole>()
//    .AddEntityFrameworkStores<UserDbContext>()
//    .AddDefaultTokenProviders();


//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//builder.WebHost.UseWebRoot("wwwroot");

//builder.Services.ConfigureApplicationCookie(options =>
//{
//    options.LoginPath = "/Auth/LogIn";
//    options.AccessDeniedPath = "/Auth/AccessDenied";
//    options.Cookie.SameSite = SameSiteMode.None;
//    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
//});

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//})
//    .AddCookie()
//    .AddGoogle(googleOptions =>
//    {
//        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
//        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
//    });


//app.MapHub<NotificationHub>("/notificationHub", options =>
//{
//    options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets;
//    options.CloseOnAuthenticationExpiration = true;
//});



//var app = builder.Build();

//using (var scope = app.Services.CreateScope())
//{
//    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
//    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

//    string[] roleNames = { "SuperAdmin", "Admin", "User" };

//    foreach (var role in roleNames)
//    {
//        if (!await roleManager.RoleExistsAsync(role))
//        {
//            await roleManager.CreateAsync(new IdentityRole(role));
//        }
//    }


//    string superAdminEmail = "superadmin@example.com";
//    string superAdminPassword = "SuperAdmin123!";

//    var superAdmin = await userManager.FindByEmailAsync(superAdminEmail);
//    if (superAdmin == null)
//    {
//        var newSuperAdmin = new AppUser
//        {
//            UserName = superAdminEmail,
//            Email = superAdminEmail,
//            EmailConfirmed = true,
//            FirstName = "Super",
//            LastName = "Admin"
//        };

//        var result = await userManager.CreateAsync(newSuperAdmin, superAdminPassword);
//        if (result.Succeeded)
//        {
//            if (!await userManager.IsInRoleAsync(newSuperAdmin, "SuperAdmin")) 
//            {
//                await userManager.AddToRoleAsync(newSuperAdmin, "SuperAdmin");
//            }
//        }
//    }
//    else
//    {
//        if (!await userManager.IsInRoleAsync(superAdmin, "SuperAdmin")) 
//        {
//            await userManager.AddToRoleAsync(superAdmin, "SuperAdmin");
//        }
//    }
//}




//app.UseHsts();

//app.UseStaticFiles();


//app.UseHttpsRedirection();
//app.UseRouting();

//app.UseAuthentication();
//app.UseAuthorization();

//app.MapStaticAssets();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Auth}/{action=LogIn}/{id?}")
//    .WithStaticAssets();


//app.Run();