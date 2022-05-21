#nullable disable
using System.Diagnostics.CodeAnalysis;
using CQRS_MediatR;
using CQRS_MediatR.API.Models;
using CQRS_MediatR.BLL.Commands;
using CQRS_MediatR.BLL.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using CQRS_MediatR.API.Filters;
using AppContext = CQRS_MediatR.API.DBContext.AppContext;
using CQRS_MediatR.API.Logging;

namespace CQRS_MediatR.API.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IServiceProvider _serviceProvider;
        private readonly string viewUrl = "~/API/Views/Auth/";

        public AuthController(IMediator mediator, IServiceProvider serviceProvider)
        {
            _mediator = mediator;
            _serviceProvider = serviceProvider;
        }

        [HttpGet("login")] // GET: /auth/login
        public IActionResult LogIn()
        {
            if (User.Identity.IsAuthenticated) return Redirect("~/users");
            return View($"{viewUrl}LogIn.cshtml");
        }
        
        [HttpPost("login")] // POST: /auth/login
        public async Task<IActionResult> LogIn([FromForm] User dataUser) // Авторизация
        {
            if (!ModelState.IsValid) return BadRequest(); // Проверка модели из формы на валидность

            var user = await _mediator.Send(new GetUserByUsernameQuery(dataUser)); // Получение юзера по username

            if (user is null) return BadRequest(new { message = "Неверное имя пользователя" });

            if (!BCrypt.Net.BCrypt.EnhancedVerify(dataUser.PassHash, user.PassHash))
                    return BadRequest(new { message = "Неверный пароль пользователя" }); // Проверка соли пароля

            await _mediator.Send(new IssueTokenCommand(user, HttpContext)); // Издание токена JWT

            return Redirect("~/users");
        }

        [Authorize]
        [HttpGet("logout")] // GET: /auth/logout
        public ActionResult LogOut()
        {
            return View($"{viewUrl}LogOut.cshtml");
        }

        [Authorize]
        [HttpPost("logout")] // POST: /auth/logout
        public IActionResult LogOutConfirmed() // Выход из системы
        {
            HttpContext.Session.Clear();
            HttpContext.Response.Cookies.Delete("access_token");
            Response.StatusCode = 401;

            var user = User.Identity.Name;

            return Redirect("~/auth/login");
        }

        [HttpGet("sign-up")] // GET: /auth/sign-up
        public ActionResult SignUp()
        {
            if (User.Identity.IsAuthenticated) return Redirect("~/users");
            return View($"{viewUrl}SignUp.cshtml");
        }

        [HttpPost("sign-up")] // POST: /auth/sign-up
        public async Task<IActionResult> SignUp([FromForm] User dataUser) // Регистрация
        {
            if (!ModelState.IsValid) return BadRequest();

            if (await _mediator.Send(new GetUserByUsernameQuery(dataUser)) is not null) // Проверка занятости логина
                return BadRequest(new { message = "Имя пользователя занято!" });

            var user = await _mediator.Send(new CreateUserCommand(dataUser)); // Создание юзера и хеширование + соление пароля

            await _mediator.Send(new IssueTokenCommand(user, HttpContext)); // Издание токена JWT

            return Redirect("~/users");
        }
    }
}
