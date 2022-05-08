#nullable disable
using CQRS_MediatR;
using CQRS_MediatR.API.Models;
using CQRS_MediatR.BLL.Commands;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using AppContext = CQRS_MediatR.API.DBContext.AppContext;

namespace CQRS_MediatR.API.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly IMediator _mediator;
        private readonly string viewUrl = "~/API/Views/";

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("login")]
        public ActionResult LogIn()
        {
            return View($"{viewUrl}Auth/LogIn.cshtml");
        }

        [HttpPost("login")]
        public async Task<IActionResult> LogIn([FromForm] User dataUser)
        {
            if (!ModelState.IsValid) return BadRequest();

            Console.WriteLine(Request.Headers["Authorization"]);

            var user = await _mediator.Send(new AuthUserCommand(dataUser));

            if (user is null) return NotFound(new { message = "Неверное имя или пароль" });

            var jwtResponse = await _mediator.Send(new IssueTokenCommand(user, HttpContext));

            return Redirect("~/users");
        }

        [HttpGet("logout")]
        public ActionResult LogOut()
        {

            return View($"{viewUrl}Auth/LogOut.cshtml");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> LogOut([FromForm] User dataUser)
        {
            if (!ModelState.IsValid) return BadRequest();

            Console.WriteLine(Request.Headers["Authorization"]);

            var user = await _mediator.Send(new AuthUserCommand(dataUser));

            if (user is null) return NotFound(new { message = "Неверное имя или пароль" });

            var jwtResponse = await _mediator.Send(new IssueTokenCommand(user, HttpContext));

            return Redirect("~/users");
        }

        [HttpGet("sign-up")]
        public ActionResult SignUp()
        {
            return View($"{viewUrl}Auth/SignUp.cshtml");
        }

        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp([FromForm] User dataUser)
        {
            if (!ModelState.IsValid) return BadRequest();

            Console.WriteLine(Request.Headers["Authorization"]);

            var user = await _mediator.Send(new AuthUserCommand(dataUser));

            if (user is null) return NotFound(new { message = "Неверное имя или пароль" });

            var jwtResponse = await _mediator.Send(new IssueTokenCommand(user, HttpContext));

            return Redirect("~/users");
        }
    }
}
