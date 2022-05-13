#nullable disable
using System.Diagnostics.CodeAnalysis;
using CQRS_MediatR;
using CQRS_MediatR.API.DBContext;
using CQRS_MediatR.API.Filters;
using CQRS_MediatR.API.Models;
using CQRS_MediatR.BLL.Commands;
using CQRS_MediatR.BLL.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using AppContext = CQRS_MediatR.API.DBContext.AppContext;

namespace CQRS_MediatR.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("users")]
    public class UsersController : Controller
    {
        private readonly IMediator _mediator;
        private readonly string viewsUrl = "~/API/Views/Users/";

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet] // GET: /users
        public IActionResult Index() // "Главная" страница
        {
            var users = _mediator.Send(new GetUsersQuery()); // Получение всех юзеров

            if (users is null) return NotFound(new { message = "Пользователи не найдены" });

            return View($"{viewsUrl}Index.cshtml", users.Result.ToList());
        }

        [IdVerify]
        [HttpGet("info/{id}")] // GET: /users/info/id
        public async Task<IActionResult> Details(string id) // Информация о юзере
        {
            var user = await _mediator.Send(new GetUserByIdQuery(id)); // Получение юзера по id

            if (user is null) return NotFound(new { message = "Пользователь не найден" });

            return View($"{viewsUrl}Details.cshtml", user);
        }

        [HttpGet("create")] // GET: /users/create
        public IActionResult Create()
        {
            return View($"{viewsUrl}Create.cshtml");
        }

        [HttpPost("create")] // POST: /users/create
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] User dataUser) // Добавление юзера
        {
            if (dataUser is null) return BadRequest();

            User user = null;

            if (ModelState.IsValid)
            {
                user = await _mediator.Send(new CreateUserCommand(dataUser)); // Создание юзера и хеширование + соление пароля
                return RedirectToAction(nameof(Index));
            }

            if (user is null) return BadRequest();

            return View($"{viewsUrl}Create.cshtml", user);
        }

        [IdVerify]
        [HttpGet("edit/{id}")] // GET: /users/edit/id
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _mediator.Send(new GetUserByIdQuery(id)); // Вывод юзера, которого будем редачить

            if (user is null) return NotFound(new { message = "Пользователь не найден" });

            return View($"{viewsUrl}Edit.cshtml", user);
        }

        [IdVerify]
        [HttpPost("edit/{id}")] // POST: /users/edit/id
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [FromForm] User dataUser) // Редактирование юзера
        {
            if (dataUser is null) return BadRequest();

            if (id != dataUser.Id) return NotFound();

            User user = null!;

            if (ModelState.IsValid)
            {
                user = await _mediator.Send(new UpdateUserCommand(dataUser)); // Обновление данных в БД
                return RedirectToAction(nameof(Index));
            }

            return View($"{viewsUrl}Edit.cshtml", user);
        }
        
        [IdVerify]
        [HttpGet("delete/{id}")] // GET: /users/delete/id
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _mediator.Send(new GetUserByIdQuery(id)); // Вывод юзера, которого будем удалять

            if (user is null) return NotFound(new { message = "Пользователь не найден" });

            return View($"{viewsUrl}Delete.cshtml", user);
        }

        [IdVerify]
        [HttpPost("delete/{id}")] // POST: /users/delete/id
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _mediator.Send(new GetUserByIdQuery(id)); // Получение юзера по id

            if (user is null) return NotFound(new { message = "Пользователь не найден" });

            await _mediator.Send(new DeleteUserCommand(user)); // Удаление юзера из БД

            return RedirectToAction(nameof(Index));
        }
    }
}
