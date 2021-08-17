using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PingPong
{
    public class Date
    {
        public static DateTime TimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
    }
}
