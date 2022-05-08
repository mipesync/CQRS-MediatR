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
        private readonly string viewUrl = "~/API/Views/Auth/";

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("login")]
        public ActionResult LogIn()
        {
            return View($"{viewUrl}LogIn.cshtml");
        }

        [HttpPost("login")]
        public async Task<IActionResult> LogIn([FromForm] User dataUser)
        {
            if (!ModelState.IsValid) return BadRequest();

            var user = await _mediator.Send(new AuthUserCommand(dataUser));

            if (user is null) return BadRequest(new { message = "Неверное имя или пароль" });

            await _mediator.Send(new IssueTokenCommand(user, HttpContext));

            return Redirect("~/users");
        }

        [Authorize]
        [HttpGet("logout")]
        public ActionResult LogOutAgree()
        {
            return View($"{viewUrl}LogOut.cshtml");
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            HttpContext.Response.Cookies.Delete("access_token");
            Response.StatusCode = 401;

            return RedirectToAction(nameof(LogIn));  // УНИКАЛЬНОСТЬ ЛОГИНА, СОЗДАТЬ ФИЛЬТР ДЛЯ ЛОГИРОВАНИЯ
        }

        [HttpGet("sign-up")]
        public ActionResult SignUp()
        {
            return View($"{viewUrl}SignUp.cshtml");
        }

        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp([FromForm] User dataUser)
        {
            if (!ModelState.IsValid) return BadRequest();

            var user = await _mediator.Send(new CreateUserCommand(dataUser));

            await _mediator.Send(new IssueTokenCommand(user, HttpContext));

            return Redirect("~/users");
        }
    }
}
