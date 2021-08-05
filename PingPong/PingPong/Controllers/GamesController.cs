﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PingPong.Models;
using Dapper;
using Microsoft.Data.SqlClient;

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
        [Route("games", Name = "games")]
        public async Task<IActionResult> Index()
        {
            var pingPongContext = _context.Games.Include(g => g.TeamANavigation).Include(g => g.TeamBNavigation).Include(g => g.VictorNavigation);
            return View(await pingPongContext.ToListAsync());
        }

        // GET: Games/Details/5
        [Route("games/{id:}")]
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
        [Route("games/create")]
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
        [Route("games/create")]
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
        [Route("games/edit/{id:}")]
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
        [Route("games/edit/{id:}")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TeamA,TeamB,WinScore,LoseScore,Victor,Date")] Game game)
        {
             if (id != game.Id)
             {
                 return NotFound();
             }
            /*
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(game);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
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
            return View(game);*/

            using (var connection = new SqlConnection("Data Source=DESKTOP-4JOHSKQ;Initial Catalog=PingPong;Integrated Security=True"))
            {
                await connection.OpenAsync();
                var sqlStatement = @"
UPDATE games 
SET  team_a = " + game.TeamA +
",team_b = " + game.TeamB + ",win_score = " + game.WinScore + ",lose_score = " + game.LoseScore + ",victor = " + game.Victor + "WHERE Id = " + id;
                await connection.ExecuteAsync(sqlStatement,game);
            }
            return Ok();
        }

        // GET: Games/Delete/5
        [Route("games/delete/{id:}")]
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
        [Route("games/delete/{id:}")]
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
