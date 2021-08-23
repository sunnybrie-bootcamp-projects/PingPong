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
            
            return View("Index");
        }

    }
}
