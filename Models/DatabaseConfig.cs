using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary
{
    public class DatabaseConfig
    {
        public DatabaseType DatabaseType { get; set; }
        public string ConnectionString { get; set; }
        public int CommandTimeout { get; set; }
        public string LogPath { get; set; }

        public DatabaseConfig()
        {
            CommandTimeout = 30;
            LogPath = AppDomain.CurrentDomain.BaseDirectory + "\\logs\\dataaccess.log";
        }

        public DatabaseConfig(DatabaseType databaseType, string connectionString)
        {
            DatabaseType = databaseType;
            ConnectionString = connectionString;
            CommandTimeout = 30;
            LogPath = AppDomain.CurrentDomain.BaseDirectory + "\\logs\\dataaccess.log";
        }
    }
}
