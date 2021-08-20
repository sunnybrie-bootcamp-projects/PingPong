using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PingPong.Models;
using Microsoft.Data.SqlClient;
using Dapper;
using Dapper.Contrib.Extensions;
using System.Data;

namespace PingPong.Controllers
{
    public class GamesController : Controller
    {
        private readonly PingPongContext _context;

        public IEnumerable<Game> Games { get; private set; }

        public GamesController(PingPongContext context)
        {
            _context = context;

            using (IDbConnection connection = new SqlConnection("Data Source=DESKTOP-4JOHSKQ;Initial Catalog=PingPong;Integrated Security=True"))
            {
                connection.Open();

                var queryString = $"SELECT g.id AS Id," +
                        "g.date AS Date, " +
                        "g.team_a AS TeamAId, " +
                        "a.teamname AS TeamA, " +
                        "g.team_b AS TeamBId, " +
                        "b.teamname AS TeamB, " +
                        "g.victor AS VictorId, " +
                        "v.teamname AS Victor, " +
                        "g.win_score AS WinScore, " +
                        "g.lose_score AS LoseScore " +
                      "FROM games AS g " +
                    "INNER " +
                      "JOIN teams AS a " +
                        "ON a.id = g.team_a " +
                    "INNER " +
                      "JOIN teams AS b " +
                        "ON b.id = g.team_b " +
                    "INNER " +
                      "JOIN teams AS v " +
                        "ON v.id = g.victor;";

                var gList = connection.Query<Game>(queryString);
                gList = gList.OrderByDescending(o => o.Date).ToList();

                Games = gList;
            }


        }



        // GET: Games
        [Route("games", Name = "games")]
        public async Task<IActionResult> Index()
        {
            return View(Games);

            /*using (IDbConnection connection = new SqlConnection("Data Source=DESKTOP-4JOHSKQ;Initial Catalog=PingPong;Integrated Security=True"))
            {
                connection.Open();

                var queryString = $"SELECT g.id AS Id," +
                        "g.date AS Date, " +
                        "g.team_a AS TeamAId, " +
                        "a.teamname AS TeamA, " +
                        "g.team_b AS TeamBId, " +
                        "b.teamname AS TeamB, " +
                        "g.victor AS VictorId, " +
                        "v.teamname AS Victor, " +
                        "g.win_score AS WinScore, " +
                        "g.lose_score AS LoseScore " +
                      "FROM games AS g " +
                    "INNER " +
                      "JOIN teams AS a " +
                        "ON a.id = g.team_a " +
                    "INNER " +
                      "JOIN teams AS b " +
                        "ON b.id = g.team_b " +
                    "INNER " +
                      "JOIN teams AS v " +
                        "ON v.id = g.victor;";

                var gList = connection.Query<Game>(queryString);
                gList = gList.OrderByDescending(o => o.Date).ToList();

                this.games = gList;

                return View(gList);
            }*/

            //Entity Framework Method (working)
            /*var pingPongContext = _context.Games.Include(g => g.TeamANavigation).Include(g => g.TeamBNavigation).Include(g => g.VictorNavigation);
            List<Game> gamesList = await pingPongContext.ToListAsync();

            return View(gamesList);*/

            //Dapper.Contrib GetAll() Method (not working, doesn't return all info?)
            /*using (IDbConnection connection = new SqlConnection("Data Source=DESKTOP-4JOHSKQ;Initial Catalog=PingPong;Integrated Security=True"))
            {
                connection.Open();

                //var games = connection.GetAll<Game>()
                //    .ToList();

                var queryString = $"SELECT g.id AS Id," +
                        "g.date AS Date, " +
                        "g.team_a AS TeamAId, " +
                        "a.teamname AS TeamA, " +
                        "g.team_b AS TeamBId, " +
                        "b.teamname AS TeamB, " +
                        "g.victor AS VictorId, " +
                        "v.teamname AS Victor, " +
                        "g.win_score AS WinScore, " +
                        "g.lose_score AS LoseScore " +
                      "FROM games AS g " +
                    "INNER " +
                      "JOIN teams AS a " +
                        "ON a.id = g.team_a " +
                    "INNER " +
                      "JOIN teams AS b " +
                        "ON b.id = g.team_b " +
                    "INNER " +
                      "JOIN teams AS v " +
                        "ON v.id = g.victor;";

                var gamesList = connection.Query<Game>(queryString);

                return View(gamesList);
            }*/

            //Dapper QueryMultiple() Method (not working, doesn't return all column info)
            /* using (var connection = new SqlConnection("Data Source=DESKTOP-4JOHSKQ;Initial Catalog=PingPong;Integrated Security=True"))
             {
                 connection.Open();
                 using var multi = connection.QueryMultiple("SELECT * FROM games;");
                 var gamesList = multi.Read<Game>().ToList();
                 return View(gamesList);
             }*/
        }

        // GET: Games/Details/5
        [Route("games/{id:}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = Games.Where(g => g.Id == id).ToList().First();
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // GET: Games/Create
        [Route("games/create/{id:}")]
        public IActionResult Create(int id)
        {
            using (IDbConnection connection = new SqlConnection("Data Source=DESKTOP-4JOHSKQ;Initial Catalog=PingPong;Integrated Security=True"))
            {
                connection.Open();

                var queryString = (id == 1) ? "SELECT id AS Id, teamname AS Teamname FROM teams WHERE player_b IS NULL;" : "SELECT id AS Id, teamname AS Teamname FROM teams WHERE player_b IS NOT NULL;";

                var tList = connection.Query<Team>(queryString);
                tList = tList.OrderBy(t => t.Teamname).ToList();

                ViewData["TeamAId"] = new SelectList(tList, "Id", "Teamname");
                ViewData["TeamBId"] = new SelectList(tList, "Id", "Teamname");
                ViewData["VictorId"] = new SelectList(tList, "Id", "Teamname");

                return View();

            }
            
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
                /*_context.Add(game);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));*/

                using (var connection = new SqlConnection("Data Source=DESKTOP-4JOHSKQ;Initial Catalog=PingPong;Integrated Security=True"))
                {
                    connection.Open();
                    var sqlStatement = $"INSERT INTO games (team_a, team_b,win_score,lose_score,victor) VALUES({game.TeamA},{game.TeamB},{game.WinScore},{game.LoseScore},{game.Victor})";
                    connection.Execute(sqlStatement);
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["TeamAId"] = new SelectList(_context.Teams, "Id", "Teamname", game.TeamA);
            ViewData["TeamBId"] = new SelectList(_context.Teams, "Id", "Teamname", game.TeamB);
            ViewData["VictorId"] = new SelectList(_context.Teams, "Id", "Teamname", game.Victor);
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

            using (var connection = new SqlConnection("Data Source=DESKTOP-4JOHSKQ;Initial Catalog=PingPong;Integrated Security=True"))
            {
                await connection.OpenAsync();
                var sqlStatement = $"UPDATE games SET  team_a = {game.TeamA},team_b = {game.TeamB},win_score = {game.WinScore},lose_score = {game.LoseScore},victor = {game.Victor} WHERE Id = {id}";
                await connection.ExecuteAsync(sqlStatement,game);
            }
            return RedirectToAction(nameof(Index)); 
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
                .Include(g => g.TeamA)
                .Include(g => g.TeamB)
                .Include(g => g.Victor)
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
