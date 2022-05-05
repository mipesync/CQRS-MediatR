#nullable disable
using CQRS_MediatR.API.BLL.Commands;
using CQRS_MediatR.API.BLL.Queries;
using CQRS_MediatR.API.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using AppContext = CQRS_MediatR.API.DBContext.AppContext;

namespace CQRS_MediatR.API.Controllers
{
    [ApiController]
    [Route("users")]

    public class UsersController : Controller
    {
        private readonly AppContext _context;
        private readonly IMediator _mediator;
        private readonly string _connection = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json")
            .Build().GetSection("ConnectionStrings:Sqlite").Value;

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
        public async Task<IActionResult> Edit(string id, [FromForm] User user)
        {
            if (id != user.Id) return NotFound();

            if (ModelState.IsValid)
            {
                user.PassHash = BCrypt.Net.BCrypt.EnhancedHashPassword(user.PassHash);
                _context.Update(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        [HttpGet("delete/{id}")] // GET: /users/delete/id
        public async Task<IActionResult> Delete(string id)
        {
            if (id is null) return NotFound();

            var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);

            if (user is null) return NotFound();

            return View(user);
        }

        [HttpPost("delete/{id}")] //users/delete/id
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _context.Users.FindAsync(id);

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
