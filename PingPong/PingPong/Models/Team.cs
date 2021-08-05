using System;
using System.Collections.Generic;

#nullable disable

namespace PingPong.Models
{
    public partial class Team
    {
        public Team()
        {
            GameTeamANavigations = new HashSet<Game>();
            GameTeamBNavigations = new HashSet<Game>();
            GameVictorNavigations = new HashSet<Game>();
        }

        public int Id { get; set; }
        public string Teamname { get; set; }
        public byte[] DateFormed { get; set; }
        public int PlayerA { get; set; }
        public int? PlayerB { get; set; }

        public virtual Player PlayerANavigation { get; set; }
        public virtual Player PlayerBNavigation { get; set; }
        public virtual ICollection<Game> GameTeamANavigations { get; set; }
        public virtual ICollection<Game> GameTeamBNavigations { get; set; }
        public virtual ICollection<Game> GameVictorNavigations { get; set; }
    }
}
