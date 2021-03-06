using CQRS_MediatR.API;
using CQRS_MediatR.API.Logging;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using ILogger = CQRS_MediatR.API.Logging.ILogger;
using AppContext = CQRS_MediatR.API.DBContext.AppContext;

var builder = WebApplication.CreateBuilder(args);

var connection = builder.Configuration.GetConnectionString("Sqlite");

builder.Services.AddDbContext<AppContext>(options => options.UseSqlite(connection));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = AuthOptions.ISSUER,
            ValidateAudience = true,
            ValidAudience = AuthOptions.AUDIENCE,
            ValidateLifetime = true,
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            ValidateIssuerSigningKey = true
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddControllers(options => options.Filters.Add(typeof(Logging)));
builder.Services.AddTransient<ConsoleLogger>();
builder.Services.AddTransient<FileLogger>();
builder.Services.AddTransient<CombinedLogger>();

builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddMvc().WithRazorPagesRoot("/API/Views");

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseSession();

app.UseStatusCodePages(async context =>
{
    var response = context.HttpContext.Response;

    if (response.StatusCode == 401 || response.StatusCode == 403) response.Redirect("/auth/login");

    if (response.StatusCode == 404) response.Redirect("/error");
});

app.Use(async (context, next) =>
{
    var access_token = context.Request.Cookies["access_token"];
    if (!string.IsNullOrEmpty(access_token))
    {
        context.Request.Headers.Add("Authorization", "Bearer " + access_token);
    }
    await next();
});

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    await next.Invoke();
    var statusCode = context.Response.StatusCode;
});

app.MapControllers();

app.Run();