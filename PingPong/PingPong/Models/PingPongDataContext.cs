using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PingPong.Models
{
    public class PingPongDataContext : DbContext
    {
        public DbSet<Player> players { get; set; }

        public PingPongDataContext(DbContextOptions<PingPongDataContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
