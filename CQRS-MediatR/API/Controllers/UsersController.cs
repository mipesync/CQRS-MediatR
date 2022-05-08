﻿#nullable disable
using CQRS_MediatR;
using CQRS_MediatR.API.DBContext;
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
        public IActionResult Index()
        {
            var users = _mediator.Send(new GetUsersQuery());

            return View($"{viewsUrl}Index.cshtml", users.Result.ToList());
        }

        [HttpGet("info/{id}")] // GET: /users/info/id
        public async Task<IActionResult> Details(string id)
        {
            if (id is null) return BadRequest();

            var user = await _mediator.Send(new GetUserByIdQuery(id));

            return View($"{viewsUrl}Details.cshtml", user);
        }

        [HttpGet("create")] // GET: /users/create
        public IActionResult Create()
        {
            return View($"{viewsUrl}Create.cshtml");
        }

        [HttpPost("create")] // POST: /users/create
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] User dataUser)
        {
            User user = null;
            if (ModelState.IsValid)
            {
                user = await _mediator.Send(new CreateUserCommand(dataUser));
                return RedirectToAction("Index");
            }
            return View($"{viewsUrl}Create.cshtml", user);
        }

        [HttpGet("edit/{id}")] // GET: /users/edit/id
        public async Task<IActionResult> Edit(string id)
        {
            if (id is null) return BadRequest();

            var user = await _mediator.Send(new GetUserByIdQuery(id));

            return View($"{viewsUrl}Edit.cshtml", user);
        }

        [HttpPost("edit/{id}")] // POST: /users/edit/id
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [FromForm] User dataUser)
        {
            if (id != dataUser.Id) return NotFound();
            User user = null!;

            if (ModelState.IsValid)
            {
                user = await _mediator.Send(new UpdateUserCommand(dataUser));
                return RedirectToAction("Index");
            }
            return View($"{viewsUrl}Edit.cshtml", user);
        }

        [HttpGet("delete/{id}")] // GET: /users/delete/id
        public async Task<IActionResult> Delete(string id)
        {
            if (id is null) return BadRequest();

            var user = await _mediator.Send(new GetUserByIdQuery(id));

            return View($"{viewsUrl}Delete.cshtml", user);
        }

        [HttpPost("delete/{id}")] // POST: /users/delete/id
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (id is null) return BadRequest();

            var user = await _mediator.Send(new GetUserByIdQuery(id));

            await _mediator.Send(new DeleteUserCommand(user));

            return RedirectToAction("Index");
        }
    }
}