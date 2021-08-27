using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PingPong.Models
{
    public class StatsBoard
    {
        public StatsBoard(int id, bool isPlayer)
        {
            using (IDbConnection connection = new SqlConnection("Data Source=DESKTOP-4JOHSKQ;Initial Catalog=PingPong;Integrated Security=True"))
            {
                connection.Open();

                //For getting all of player's team games or just getting all of a team's games
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

                //If player, get and set extra stats
                if (isPlayer)
                {
                    //For getting all teams the player is on
                    var teamQueryString = $"SELECT id AS Id, teamname AS Teamname FROM teams WHERE player_b IS NOT NULL AND player_a = {id} OR player_b = {id};";
                    //For getting player's "single" team id
                    var singleQueryString = $"SELECT id FROM teams WHERE player_b IS NULL AND player_a = {id};";

                    //Get all teams player is on
                    var teamsList = connection.Query(teamQueryString);
                    teamsList = teamsList.OrderByDescending(teams => teams.Teamname).ToList();

                    //Get player's "single" team id
                    var singleTeamId = connection.Query(singleQueryString).First().id;

                    //All ids that include the player
                    List<int> allIds = new List<int>();
                    allIds.Add(singleTeamId);

                    //Add all team ids to games query
                    foreach (var team in teamsList)
                    {
                        allIds.Add(team.Id);
                        teamGameQueryString += $" g.team_a = {team.Id} OR g.team_b = {team.Id}";

                        if (team == teamsList.Last())
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
                    TeamWins = teamGamesList.Count(g => allIds.Contains((int)g.VictorId));
                    TeamLosses = teamGamesList.Count(g => !allIds.Contains((int)g.VictorId));
                    SingleWins = singleGamesList.Count(g => allIds.Contains((int)g.VictorId));
                    SingleLosses = singleGamesList.Count(g => !allIds.Contains((int)g.VictorId));
                    TotalWins = (int)TeamWins + (int)SingleWins;
                    TotalLosses = (int)TeamLosses + (int)SingleLosses;

                    
                    TeamWinPercentage = getPercentage((float)TeamWins, (float)TeamLosses);
                    TeamWinLoseRatio = reduceFraction((int)TeamWins, (int)TeamLosses);
                    SingleWinPercentage = getPercentage((float)SingleWins, (float)SingleLosses);
                    SingleWinLoseRatio = reduceFraction((int)SingleWins, (int)SingleLosses);
                    WinPercentage = getPercentage((float)TotalWins, (float)TotalLosses);
                    WinLoseRatio = reduceFraction(TotalWins, TotalLosses);

                    RecentGames = singleGamesList.Concat(teamGamesList).OrderByDescending(g => g.Date).Take(5).ToList();

                }
                else
                {
                    teamGameQueryString += $" g.team_a = {id} OR g.team_b = {id};";

                    var gamesList = connection.Query<Game>(teamGameQueryString);

                    TotalWins = gamesList.Count(g => g.VictorId == id);
                    TotalLosses = gamesList.Count(g => g.VictorId != id);
                    WinPercentage = getPercentage((float)TotalWins, (float)TotalLosses);
                    WinLoseRatio = reduceFraction(TotalWins, TotalLosses);

                    RecentGames = gamesList.OrderByDescending(g => g.Date).Take(5).ToList();
                }

            }

        }
        public float WinPercentage { get; set; }
        public int TotalWins { get; set; }
        public int TotalLosses { get; set; }
        public string WinLoseRatio { get; set; }
        public List<Game> RecentGames { get; set; }

        public int? SingleWins {get; set;}
        public int? SingleLosses { get; set; }
        public float? SingleWinPercentage { get; set; }
        public string? SingleWinLoseRatio { get; set; }
        public int? TeamWins { get; set; }
        public int? TeamLosses { get; set; }
        public float? TeamWinPercentage { get; set; }
        public string? TeamWinLoseRatio { get; set; }

        private static string reduceFraction(int x, int y)
        {
            int d;
            d = __gcd(x, y);

            if (d != 0)
            {
                x = x / d;
                y = y / d;
            }

            return $"{x}:{y}";
        }

        private static int __gcd(int a, int b)
        {
            if (b == 0)
                return a;
            return __gcd(b, a % b);

        }

        private static float getPercentage (float wins, float losses)
        {
            return (float)((wins / (losses + wins)) * 100.00);
        }
    }


}