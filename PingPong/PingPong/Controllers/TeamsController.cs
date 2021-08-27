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
    public class TeamsController : Controller
    {
        private readonly PingPongContext _context;

        public IEnumerable<Team> Teams { get; private set; }

        public TeamsController(PingPongContext context)
        {
            _context = context;

            using (IDbConnection connection = new SqlConnection("Data Source=DESKTOP-4JOHSKQ;Initial Catalog=PingPong;Integrated Security=True"))
            {
                _context = context;
                connection.Open();

                var queryString = $"SELECT t.id AS Id, t.teamname AS Teamname, t.date_formed AS DateFormed, t.player_a AS PlayerAId, a.username AS PlayerA, t.player_b AS PlayerBId, b.username AS PlayerB FROM teams AS t INNER JOIN players AS a ON a.id = t.player_a INNER JOIN players AS b ON b.id = t.player_b WHERE t.player_b IS NOT NULL; ";

                var tList = connection.Query<Team>(queryString);
                tList = tList.OrderByDescending(t => t.Teamname).ToList();

                Teams = tList;
            }


        }

        // GET: Teams
        [Route("teams", Name = "teams")]
        public async Task<IActionResult> Index()
        {
            return View(Teams);
        }

        // GET: Teams/Details/5
        [Route("teams/{id:}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Team team = Teams.Where(t => t.Id == id).First();

            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        // GET: Teams/Create
        [Route("teams/create")]
        public IActionResult Create()
        {
            ViewData["PlayerA"] = new SelectList(_context.Players, "Id", "FirstName");
            ViewData["PlayerB"] = new SelectList(_context.Players, "Id", "FirstName");
            return View();
        }

        // POST: Teams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Teamname,DateFormed,PlayerA,PlayerB")] Team team)
        {
            if (ModelState.IsValid)
            {
                _context.Add(team);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PlayerA"] = new SelectList(_context.Players, "Id", "FirstName", team.PlayerA);
            ViewData["PlayerB"] = new SelectList(_context.Players, "Id", "FirstName", team.PlayerB);
            return View(team);
        }

        // GET: Teams/Edit/5
        [Route("teams/edit/{id:}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams.FindAsync(id);
            if (team == null)
            {
                return NotFound();
            }
            ViewData["PlayerA"] = new SelectList(_context.Players, "Id", "FirstName", team.PlayerA);
            ViewData["PlayerB"] = new SelectList(_context.Players, "Id", "FirstName", team.PlayerB);
            return View(team);
        }

        // POST: Teams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("teams/edit/{id:}")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Teamname,DateFormed,PlayerA,PlayerB")] Team team)
        {
            if (id != team.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(team);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeamExists(team.Id))
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
            ViewData["PlayerA"] = new SelectList(_context.Players, "Id", "FirstName", team.PlayerA);
            ViewData["PlayerB"] = new SelectList(_context.Players, "Id", "FirstName", team.PlayerB);
            return View(team);
        }

        // GET: Teams/Delete/5
        [Route("teams/delete/{id:}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams
                .Include(t => t.PlayerA)
                .Include(t => t.PlayerB)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        // POST: Teams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("teams/delete/{id:}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var team = await _context.Teams.FindAsync(id);
            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeamExists(int id)
        {
            return _context.Teams.Any(e => e.Id == id);
        }
    }
}
