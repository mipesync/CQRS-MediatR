#nullable disable
using CQRS_MediatR.BLL.Commands;
using CQRS_MediatR.BLL.Queries;
using CQRS_MediatR.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using AppContext = CQRS_MediatR.DBContext.AppContext;

namespace CQRS_MediatR.Controllers
{
    [ApiController]
    [Route("users")]

    public class UsersController : Controller
    {
        private readonly AppContext _context;
        private readonly IMediator _mediator;

        public UsersController(AppContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        [HttpGet("get")] // GET: /users/get
        public IActionResult Index()
        {
            var users = _mediator.Send(new GetUsersQuery());

            return View(users.Result.ToList());
        }

        [HttpGet("info/{id}")] // GET: /users/info/id
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) { return NotFound(); }

            var user = await _mediator.Send(new GetUserByIdQuery(id));

            return View(user);
        }

        [HttpGet("create")] // GET: /users/create
        public IActionResult Create()
        {
            return View();
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
            return View(user);
        }

        [HttpGet("edit/{id}")] // GET: /users/edit/id
        public async Task<IActionResult> Edit(string id)
        {
            if (id is null) return NotFound();

            var user = await _mediator.Send(new GetUserByIdQuery(id));

            return View(user);
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
            return View(user);
        }

        [HttpGet("delete/{id}")] // GET: /users/delete/id
        public async Task<IActionResult> Delete(string id)
        {
            if (id is null) return NotFound();

            var user = await _mediator.Send(new GetUserByIdQuery(id));

            return View(user);
        }

        [HttpPost("delete/{id}")] // POST: /users/delete/id
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _mediator.Send(new DeleteUserCommand(id));

            return RedirectToAction("Index");
        }
    }
}
