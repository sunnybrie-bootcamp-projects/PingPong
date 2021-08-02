using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data.SqlClient;
using PingPong.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.IdentityModel.Protocols;

namespace PingPong
{
    public class HomeController : Controller
    {
        private readonly PingPongContext _db;

        public HomeController(PingPongContext db)
        {
            _db = db;
        }

        [Route("", Name = "home")]
        public IActionResult Index()
        {
            var dataArr = _db.Players.ToArray();

            List<Player> playerList = new List<Player>();

            for(int i = 0; i < dataArr.Length; i++)
            {
                Player player = dataArr[i];
                playerList.Add(player);
            }
            
            return View("Index", playerList.First());
        }

        
        public IActionResult PlayerBoard()
        {
            var dataArr = _db.Players.ToArray();

            List<Player> playerList = new List<Player>();

            for (int i = 0; i < dataArr.Length; i++)
            {
                Player player = dataArr[i];
                playerList.Add(player);
            }

            return View("Players", playerList);
        }
    }
}
