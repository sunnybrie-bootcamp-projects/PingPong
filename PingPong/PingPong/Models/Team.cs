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
        public int PlayerAId { get; set; }
        public int? PlayerBId { get; set; }
        public DateTime? DateFormed { get; set; }

        public virtual Player PlayerA { get; set; }
        public virtual Player PlayerB { get; set; }
        public virtual ICollection<Game> GameTeamANavigations { get; set; }
        public virtual ICollection<Game> GameTeamBNavigations { get; set; }
        public virtual ICollection<Game> GameVictorNavigations { get; set; }
    }
}
