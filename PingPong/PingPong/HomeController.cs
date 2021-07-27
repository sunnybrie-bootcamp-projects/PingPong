using Microsoft.AspNetCore.Mvc;
using PingPong.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PingPong
{
    public class HomeController : Controller
    {
        private readonly PingPongDataContext _db;

        public HomeController(PingPongDataContext db)
        {
            _db = db;
        }

        [Route("")]
        public IActionResult Index()
        {
            Player[] playerList = _db.players.ToArray();
            Console.WriteLine(playerList);
            return View(playerList);
        }
    }
}
