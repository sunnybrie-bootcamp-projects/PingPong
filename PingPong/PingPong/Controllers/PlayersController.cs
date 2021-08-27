using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PingPong.Models;

namespace PingPong.Controllers
{
    public class PlayersController : Controller
    {
        private readonly PingPongContext _context;

        public IEnumerable<Player> Players { get; private set; }

        public PlayersController(PingPongContext context)
        {
            _context = context;

            using (IDbConnection connection = new SqlConnection("Data Source=DESKTOP-4JOHSKQ;Initial Catalog=PingPong;Integrated Security=True"))
            {
                connection.Open();

                var queryString = $"SELECT id AS Id, username AS Username, first_name AS FirstName, last_name AS LastName, date_joined AS DateJoined, image_url AS ImageUrl FROM players;";
                var pList = connection.Query<Player>(queryString);
                pList = pList.OrderByDescending(p => p.Username).ToList();

                Players = pList;
            }
        }

        // GET: Players
        [Route("players", Name = "players")]
        public async Task<IActionResult> Index()
        {
            return View(Players);
        }

        // GET: Players/Details/5
        [Route("players/{id:}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var player = Players.Where(g => g.Id == id).ToList().First();
            if (player == null)
            {
                return NotFound();
            }

            return View(player);
        }

        // GET: Players/Create
        [Route("players/create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Players/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("players/create")]
        public async Task<IActionResult> Create([Bind("Id,Username,FirstName,LastName,DateJoined,ImageUrl")] Player player)
        {
            if (ModelState.IsValid)
            {
                _context.Add(player);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(player);
        }

        // GET: Players/Edit/5
        [Route("players/edit/{id:}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var player = await _context.Players.FindAsync(id);
            if (player == null)
            {
                return NotFound();
            }
            return View(player);
        }

        // POST: Players/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("players/edit/{id:}")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Username,FirstName,LastName,DateJoined,ImageUrl")] Player player)
        {
            if (id != player.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(player);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlayerExists(player.Id))
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
            return View(player);
        }

        // GET: Players/Delete/5
        [Route("players/delete/{id:}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var player = await _context.Players
                .FirstOrDefaultAsync(m => m.Id == id);
            if (player == null)
            {
                return NotFound();
            }

            return View(player);
        }

        // POST: Players/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("players/delete/{id:}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var player = await _context.Players.FindAsync(id);
            _context.Players.Remove(player);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlayerExists(int id)
        {
            return _context.Players.Any(e => e.Id == id);
        }
    }
}
