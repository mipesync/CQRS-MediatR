#nullable disable
using CQRS_MediatR;
using CQRS_MediatR.API.Models;
using CQRS_MediatR.BLL.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using AppContext = CQRS_MediatR.API.DBContext.AppContext;

namespace CQRS_MediatR.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly AppContext _context;
        private readonly IMediator _mediator;
        private string _connection = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json")
            .Build().GetSection("ConnectionStrings:Sqlite").Value;

        public AuthController(AppContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        [HttpGet("login")]
        public ActionResult LogIn()
        {
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> LogIn([FromForm] User dataUser)
        {
            if (!ModelState.IsValid) return BadRequest();

            var user = await _mediator.Send(new AuthUserCommand(dataUser));

            if (user is null) return NotFound(new { message = "Неверное имя или пароль" });

            var token = await _mediator.Send(new IssueTokenCommand(dataUser));

            return View(token);
        }

    }
}
