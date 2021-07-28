using System;
using System.Collections.Generic;

#nullable disable

namespace PingPong.Models
{
    public partial class Team
    {
        public int Id { get; set; }
        public string Teamname { get; set; }
        public byte[] DateFormed { get; set; }
        public int PlayerA { get; set; }
        public int? PlayerB { get; set; }

        public virtual Player PlayerANavigation { get; set; }
        public virtual Player PlayerBNavigation { get; set; }
    }
}
