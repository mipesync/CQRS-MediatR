using CQRS_MediatR;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using Microsoft.Data.Sqlite;
using AppContext = CQRS_MediatR.DBContext.AppContext;

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
builder.Services.AddMvc();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();