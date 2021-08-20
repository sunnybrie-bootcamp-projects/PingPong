using System;
using System.Collections.Generic;

#nullable disable

namespace PingPong.Models
{
    public partial class Game
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public int TeamAId { get; set; }
        public int TeamBId { get; set; }
        public int? WinScore { get; set; }
        public int? LoseScore { get; set; }
        public int? VictorId { get; set; }
        
        public string TeamA { get; set; }
        public string TeamB { get; set; }
        public string Victor { get; set; }
    }
}
