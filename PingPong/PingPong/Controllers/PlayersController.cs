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

                var queryString = $"SELECT id AS Id, username AS Username, first_name AS FirstName, last_name AS LastName, date_joined AS DateJoined, image_url AS ImageUrl FROM players; ";
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

            using (IDbConnection connection = new SqlConnection("Data Source=DESKTOP-4JOHSKQ;Initial Catalog=PingPong;Integrated Security=True"))
            {
                connection.Open();

                //For getting all teams the player is on
                var teamQueryString = $"SELECT id AS Id, teamname AS Teamname FROM teams WHERE player_b IS NOT NULL AND player_a = {player.Id} OR player_b = {player.Id};";
                //For getting player's "single" team id
                var singleQueryString = $"SELECT id FROM teams WHERE player_b IS NULL AND player_a = {player.Id};";

                //Get all teams player is on
                var teamsList = connection.Query(teamQueryString);
                teamsList = teamsList.OrderByDescending(teams => teams.Teamname).ToList();

                //Get player's "single" team id
                var singleTeamId = connection.Query(singleQueryString).First().id;

                List<int> allIds = new List<int>();
                allIds.Add(singleTeamId);

                //For getting all of player's team games
                //I know this is horrendous.
                //Need to look up how to save the data in the server cache so I don't have to repeat queries
                //I already did in games controller
                var teamGameQueryString = $"SELECT g.id AS Id, " +
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
                        "ON v.id = g.victor " +
                        "WHERE";

                //Add all team ids to games query
                foreach(var team in teamsList)
                {
                    allIds.Add(team.Id);
                    teamGameQueryString += $" g.team_a = {team.Id} OR g.team_b = {team.Id}";

                    if(team == teamsList.Last())
                    {
                        teamGameQueryString += ";";
                    }
                    else
                    {
                        teamGameQueryString += " OR ";
                    }
                }

                //List of team games
                var teamGamesList = connection.Query<Game>(teamGameQueryString);

                //For getting singles games
                var singleGameQueryString = $"SELECT g.id AS Id, " +
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
                        "ON v.id = g.victor " +
                        $"WHERE g.team_a = {singleTeamId} OR g.team_b = {singleTeamId};";

                //List of singles games
                var singleGamesList = connection.Query<Game>(singleGameQueryString);

                //Get win and loss counts
                int playerTeamWins = teamGamesList.Count(g => allIds.Contains((int)g.VictorId));
                int playerTeamLosses = teamGamesList.Count(g => !allIds.Contains((int)g.VictorId));
                int playerSingleWins = singleGamesList.Count(g => allIds.Contains((int)g.VictorId));
                int playerSingleLosses = singleGamesList.Count(g => !allIds.Contains((int)g.VictorId));
                int playerTotalWins = playerTeamWins + playerSingleWins;
                int playerTotalLosses = playerTeamLosses + playerSingleLosses;

                var playerWinRatio = new { teamPercentage = (float)((float)playerTeamWins / (float)(playerTeamLosses + (float)playerTeamWins) * 100.00),
                    teamReducedTotal = $"{reduceFraction(playerTeamWins, playerTeamLosses)}",
                    teamWins = playerTeamWins,
                    teamLosses = playerTeamLosses,
                    singlePercentage = (float)((float)playerSingleWins / (float)(playerSingleLosses + (float)playerSingleWins) * 100.00),
                    singleReducedTotal = $"{reduceFraction(playerSingleWins, playerSingleLosses)}",
                    singleWins = playerSingleWins,
                    singleLosses = playerSingleLosses,
                    totalPercentage = (float)((float)playerTotalWins / (float)(playerTotalLosses + (float)playerTotalWins) * 100.00),
                    totalReducedTotal = $"{reduceFraction(playerTotalWins, playerTotalLosses)}",
                    totalWins = playerTotalWins,
                    totalLosses = playerTotalLosses,
                };

                ViewBag.playerWinRatio = playerWinRatio;
                ViewBag.teamGames = teamGamesList;

                static string reduceFraction(int x, int y)
                {
                    int d;
                    d = __gcd(x, y);

                    x = x / d;
                    y = y / d;

                    return $"{x}:{y}";
                }

                static int __gcd(int a, int b)
                {
                    if (b == 0)
                        return a;
                    return __gcd(b, a % b);

                }
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
