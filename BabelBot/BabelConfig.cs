using System;
using System.Collections.Generic;
using System.Text;

namespace BabelBot
{
    public class Database
    {
        public string ConnectionString { get; set; }
    }
    public class BabelConfig
    {
        public string Token { get; set; }
        public string SqlUsername { get; set; }
        public string SqlPassword { get; set; }
        public string SqlIp { get; set; }
        public string SqlDatabase { get; set; }
        public Database Database { get; set; }
    }
}
