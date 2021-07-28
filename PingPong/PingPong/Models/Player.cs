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

        public int Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte[] DateJoined { get; set; }

        public virtual ICollection<Team> TeamPlayerANavigations { get; set; }
        public virtual ICollection<Team> TeamPlayerBNavigations { get; set; }
    }
}
