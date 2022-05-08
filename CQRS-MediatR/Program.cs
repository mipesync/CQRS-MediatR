using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using Microsoft.Data.Sqlite;
using AppContext = CQRS_MediatR.API.DBContext.AppContext;
using CQRS_MediatR.API;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Authentication.Cookies;

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
builder.Services.AddControllers();
builder.Services.AddDistributedMemoryCache();

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

app.Use(async (context, next) =>
{
    var access_token = context.Session.GetString("access_token");
    if (!string.IsNullOrEmpty(access_token))
    {
        context.Request.Headers.Add("Authorization", "Bearer " + access_token);
    }
    await next();
});

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();