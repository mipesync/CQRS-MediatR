﻿#nullable disable
using CQRS_MediatR;
using CQRS_MediatR.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using AppContext = CQRS_MediatR.DBContext.AppContext;

namespace CQRS_MediatR.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class AuthController : Controller
    {
        private readonly AppContext _context;
        private string _connection = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json")
            .Build().GetSection("ConnectionStrings:Sqlite").Value;

        public AuthController(AppContext context)
        {
            _context = context;
        }

        public ActionResult Index()
        {
            return View();
        }

    }
}