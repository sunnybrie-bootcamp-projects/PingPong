using System;
using System.Collections.Generic;

#nullable disable

namespace PingPong.Models
{
    public partial class Player
    {
        public Player()
        {
            TeamPlayerANavigations = new HashSet<Team>();
            TeamPlayerBNavigations = new HashSet<Team>();
        }

        private int id;
        public int Id
        {
            get { return id; }
            set { id = value; Stats = new StatsBoard(value, true); }
        }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ImageUrl { get; set; }
        public DateTime? DateJoined { get; set; }
        public StatsBoard Stats { get; set; }

        public virtual ICollection<Team> TeamPlayerANavigations { get; set; }
        public virtual ICollection<Team> TeamPlayerBNavigations { get; set; }
    }
}
