#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CQRS_MediatR.DBContext;
using CQRS_MediatR.Models;
using AppContext = CQRS_MediatR.DBContext.AppContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.Sqlite;
using System.Configuration;

namespace CQRS_MediatR.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class UsersController : Controller
    {
        private readonly AppContext _context;
        private string _connection = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json")
            .Build().GetSection("ConnectionStrings:Sqlite").Value;

        public UsersController(AppContext context)
        {
            _context = context;
        }

        [HttpGet("Get")] // GET: /Users/Get
        public async Task<IActionResult> Index()
        {
            List<User> users = new List<User>();

            var sqlExpression = "SELECT * FROM Users";
            using (var sqliteConnection = new SqliteConnection(_connection))
            {
                sqliteConnection.Open();

                SqliteCommand sqliteCommand = new SqliteCommand(sqlExpression, sqliteConnection);
                using (var reader = sqliteCommand.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var id = reader.GetValue(0).ToString();
                            var name = reader.GetValue(1).ToString();
                            var username = reader.GetValue(2).ToString();
                            var passhash = reader.GetValue(3).ToString();

                            users.Add(new User { Id = id, Name = name, Username = username, PassHash = passhash });
                        }
                    }
                }
            }
            return View(users);
        }

        [HttpGet("Info/{userId}")] // GET: /Users/Info/id
        public async Task<IActionResult> Details(string userId)
        {
            if (userId == null) { return NotFound(); }

            User user = null;

            var sqlExpression = $"SELECT * FROM Users WHERE Id='{userId}'";
            using (var sqliteConnection = new SqliteConnection(_connection))
            {
                sqliteConnection.Open();

                SqliteCommand sqliteCommand = new SqliteCommand(sqlExpression, sqliteConnection);
                using (var reader = sqliteCommand.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var id = reader.GetValue(0).ToString();
                            var name = reader.GetValue(1).ToString();
                            var username = reader.GetValue(2).ToString();
                            var passhash = reader.GetValue(3).ToString();

                            user = new User { Id = id, Name = name, Username = username, PassHash = passhash };
                        }
                    }
                }
            }
            return View(user);
        }

        [HttpGet("Create")] // GET: /Users/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("Create")] // POST: /Users/Create
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        [HttpGet("Edit/{id}")] // GET: /Users/Edit/id
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost("Edit/{id}")] // POST: /Users/Edit/id
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [FromForm] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        [HttpGet("Delete/{id}")] // GET: /Users/Delete/id
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost("Delete/{id}")] //Users/Delete/id
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
