using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace DataAccessLibrary
{
    public static class DataAccessFactory
    {
        public static IDataAccess Create(DatabaseType databaseType, string connectionString, string logPath = null, int commandTimeout = 30)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string is required.");

            return new DataAccess(databaseType, connectionString, logPath, commandTimeout);
        }

        public static IDataAccess Create(DatabaseConfig config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            if (string.IsNullOrWhiteSpace(config.ConnectionString))
                throw new ArgumentException("Connection string is required.");

            return Create(config.DatabaseType, config.ConnectionString, config.LogPath, config.CommandTimeout);
        }
    }
}
