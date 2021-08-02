using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PingPong.Models;

namespace PingPong.Controllers
{
    public class GamesController : Controller
    {
        private readonly PingPongContext _context;

        public GamesController(PingPongContext context)
        {
            _context = context;
        }

        // GET: Games
        public async Task<IActionResult> Index()
        {
            var pingPongContext = _context.Games.Include(g => g.TeamANavigation).Include(g => g.TeamBNavigation).Include(g => g.VictorNavigation);
            return View(await pingPongContext.ToListAsync());
        }

        // GET: Games/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(g => g.TeamANavigation)
                .Include(g => g.TeamBNavigation)
                .Include(g => g.VictorNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // GET: Games/Create
        public IActionResult Create()
        {
            ViewData["TeamA"] = new SelectList(_context.Teams, "Id", "Teamname");
            ViewData["TeamB"] = new SelectList(_context.Teams, "Id", "Teamname");
            ViewData["Victor"] = new SelectList(_context.Teams, "Id", "Teamname");
            return View();
        }

        // POST: Games/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TeamA,TeamB,WinScore,LoseScore,Victor,Date")] Game game)
        {
            if (ModelState.IsValid)
            {
                _context.Add(game);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TeamA"] = new SelectList(_context.Teams, "Id", "Teamname", game.TeamA);
            ViewData["TeamB"] = new SelectList(_context.Teams, "Id", "Teamname", game.TeamB);
            ViewData["Victor"] = new SelectList(_context.Teams, "Id", "Teamname", game.Victor);
            return View(game);
        }

        // GET: Games/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }
            ViewData["TeamA"] = new SelectList(_context.Teams, "Id", "Teamname", game.TeamA);
            ViewData["TeamB"] = new SelectList(_context.Teams, "Id", "Teamname", game.TeamB);
            ViewData["Victor"] = new SelectList(_context.Teams, "Id", "Teamname", game.Victor);
            return View(game);
        }

        // POST: Games/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TeamA,TeamB,WinScore,LoseScore,Victor,Date")] Game game)
        {
            if (id != game.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(game);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameExists(game.Id))
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
            ViewData["TeamA"] = new SelectList(_context.Teams, "Id", "Teamname", game.TeamA);
            ViewData["TeamB"] = new SelectList(_context.Teams, "Id", "Teamname", game.TeamB);
            ViewData["Victor"] = new SelectList(_context.Teams, "Id", "Teamname", game.Victor);
            return View(game);
        }

        // GET: Games/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(g => g.TeamANavigation)
                .Include(g => g.TeamBNavigation)
                .Include(g => g.VictorNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // POST: Games/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var game = await _context.Games.FindAsync(id);
            _context.Games.Remove(game);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GameExists(int id)
        {
            return _context.Games.Any(e => e.Id == id);
        }
    }
}
