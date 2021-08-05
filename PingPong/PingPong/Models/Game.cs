using System;
using System.Collections.Generic;

#nullable disable

namespace PingPong.Models
{
    public partial class Game
    {
        public int TeamA { get; set; }
        public int TeamB { get; set; }
        public int? WinScore { get; set; }
        public int? LoseScore { get; set; }
        public int? Victor { get; set; }
        public byte[] Date { get; set; }
        public int Id { get; set; }

        public virtual Team TeamANavigation { get; set; }
        public virtual Team TeamBNavigation { get; set; }
        public virtual Team VictorNavigation { get; set; }
    }
}
