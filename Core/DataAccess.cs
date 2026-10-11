using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace DataAccessLibrary
{
    /// <summary>
    /// Core DataAccess Class - รองรับ SQL Server, MySQL, Oracle, PostgreSQL
    /// </summary>
    public class DataAccess : IDataAccess
    {
        private DbConnection _connection;
        private readonly DatabaseType _databaseType;
        private readonly string _logPath;
        private readonly int _commandTimeout;

        public string ConnectionString { get; set; }
        public DatabaseType DatabaseType { get { return _databaseType; } }

        public DataAccess(DatabaseType databaseType, string connectionString, string logPath = null, int commandTimeout = 30)
        {
            _databaseType = databaseType;
            ConnectionString = connectionString;
            _logPath = logPath ?? AppDomain.CurrentDomain.BaseDirectory + "\\logs\\dataaccess.log";
            _commandTimeout = commandTimeout;
        }

        /// <summary>
        /// เปิด Connection
        /// </summary>
        public void Open()
        {
            try
            {
                if (_connection != null && _connection.State == ConnectionState.Open)
                    return;

                _connection = CreateConnection();
                _connection.Open();
                LogHelper.WriteLog("Connection opened successfully. Database: " + _databaseType.ToString(), null, _logPath);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Open connection failed. Database: " + _databaseType.ToString(), ex, _logPath);
                throw;
            }
        }

        /// <summary>
        /// ปิด Connection
        /// </summary>
        public void Close()
        {
            try
            {
                if (_connection != null && _connection.State != ConnectionState.Closed)
                {
                    _connection.Close();
                    LogHelper.WriteLog("Connection closed.", null, _logPath);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Close connection failed", ex, _logPath);
            }
        }

        /// <summary>
        /// สร้าง Connection ตามประเภท Database
        /// </summary>
        private DbConnection CreateConnection()
        {
            switch (_databaseType)
            {
                case DatabaseType.SqlServer:
                    return new SqlConnection(ConnectionString);
                case DatabaseType.MySql:
                    // MySQL - ต้องมี MySql.Data installed
                    try
                    {
                        Type mySqlType = Type.GetType("MySql.Data.MySqlClient.MySqlConnection, MySql.Data");
                        if (mySqlType == null)
                            throw new InvalidOperationException("MySql.Data library not found. Please install MySql.Data NuGet package.");
                        return (DbConnection)Activator.CreateInstance(mySqlType, ConnectionString);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteLog("Failed to create MySQL connection", ex, _logPath);
                        throw;
                    }
                case DatabaseType.Oracle:
                    // Oracle - ต้องมี Oracle.ManagedDataAccess installed
                    try
                    {
                        Type oracleType = Type.GetType("Oracle.ManagedDataAccess.Client.OracleConnection, Oracle.ManagedDataAccess");
                        if (oracleType == null)
                            throw new InvalidOperationException("Oracle.ManagedDataAccess library not found. Please install Oracle.ManagedDataAccess NuGet package.");
                        return (DbConnection)Activator.CreateInstance(oracleType, ConnectionString);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteLog("Failed to create Oracle connection", ex, _logPath);
                        throw;
                    }
                case DatabaseType.PostgreSql:
                    // PostgreSQL - ต้องมี Npgsql installed
                    try
                    {
                        Type npgsqlType = Type.GetType("Npgsql.NpgsqlConnection, Npgsql");
                        if (npgsqlType == null)
                            throw new InvalidOperationException("Npgsql library not found. Please install Npgsql NuGet package.");
                        return (DbConnection)Activator.CreateInstance(npgsqlType, ConnectionString);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteLog("Failed to create PostgreSQL connection", ex, _logPath);
                        throw;
                    }
                default:
                    throw new NotSupportedException("Database type " + _databaseType.ToString() + " is not supported.");
            }
        }

        /// <summary>
        /// สร้าง DbCommand
        /// </summary>
        private DbCommand CreateCommand(string sql, Dictionary<string, object> parameters = null)
        {
            if (_connection == null || _connection.State != ConnectionState.Open)
                Open();

            DbCommand cmd = _connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandTimeout = _commandTimeout;
            cmd.CommandType = CommandType.Text;

            if (parameters != null && parameters.Count > 0)
            {
                foreach (KeyValuePair<string, object> param in parameters)
                {
                    DbParameter p = cmd.CreateParameter();
                    p.ParameterName = param.Key;
                    p.Value = param.Value ?? DBNull.Value;
                    cmd.Parameters.Add(p);
                }
            }

            return cmd;
        }

        /// <summary>
        /// Execute Query และคืน DataTable
        /// </summary>
        public DataTable ExecuteQuery(string sql, Dictionary<string, object> parameters = null)
        {
            DataTable table = new DataTable();
            try
            {
                LogHelper.WriteLog("ExecuteQuery: " + sql, null, _logPath);

                using (DbCommand cmd = CreateCommand(sql, parameters))
                {
                    DbDataAdapter adapter = null;

                    switch (_databaseType)
                    {
                        case DatabaseType.SqlServer:
                            adapter = new System.Data.SqlClient.SqlDataAdapter((SqlCommand)cmd);
                            break;
                        case DatabaseType.MySql:
                            Type mySqlAdapterType = Type.GetType("MySql.Data.MySqlClient.MySqlDataAdapter, MySql.Data");
                            if (mySqlAdapterType != null)
                                adapter = (DbDataAdapter)Activator.CreateInstance(mySqlAdapterType, cmd);
                            break;
                        case DatabaseType.Oracle:
                            Type oracleAdapterType = Type.GetType("Oracle.ManagedDataAccess.Client.OracleDataAdapter, Oracle.ManagedDataAccess");
                            if (oracleAdapterType != null)
                                adapter = (DbDataAdapter)Activator.CreateInstance(oracleAdapterType, cmd);
                            break;
                        case DatabaseType.PostgreSql:
                            Type npgsqlAdapterType = Type.GetType("Npgsql.NpgsqlDataAdapter, Npgsql");
                            if (npgsqlAdapterType != null)
                                adapter = (DbDataAdapter)Activator.CreateInstance(npgsqlAdapterType, cmd);
                            break;
                    }

                    if (adapter != null)
                    {
                        adapter.Fill(table);
                        adapter.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("ExecuteQuery failed: " + sql, ex, _logPath);
                throw;
            }

            return table;
        }

        /// <summary>
        /// Execute Query และคืน DataSet
        /// </summary>
        public DataSet ExecuteDataSet(string sql, Dictionary<string, object> parameters = null)
        {
            DataSet ds = new DataSet();
            try
            {
                LogHelper.WriteLog("ExecuteDataSet: " + sql, null, _logPath);

                using (DbCommand cmd = CreateCommand(sql, parameters))
                {
                    DbDataAdapter adapter = null;

                    switch (_databaseType)
                    {
                        case DatabaseType.SqlServer:
                            adapter = new System.Data.SqlClient.SqlDataAdapter((SqlCommand)cmd);
                            break;
                        case DatabaseType.MySql:
                            Type mySqlAdapterType = Type.GetType("MySql.Data.MySqlClient.MySqlDataAdapter, MySql.Data");
                            if (mySqlAdapterType != null)
                                adapter = (DbDataAdapter)Activator.CreateInstance(mySqlAdapterType, cmd);
                            break;
                        case DatabaseType.Oracle:
                            Type oracleAdapterType = Type.GetType("Oracle.ManagedDataAccess.Client.OracleDataAdapter, Oracle.ManagedDataAccess");
                            if (oracleAdapterType != null)
                                adapter = (DbDataAdapter)Activator.CreateInstance(oracleAdapterType, cmd);
                            break;
                        case DatabaseType.PostgreSql:
                            Type npgsqlAdapterType = Type.GetType("Npgsql.NpgsqlDataAdapter, Npgsql");
                            if (npgsqlAdapterType != null)
                                adapter = (DbDataAdapter)Activator.CreateInstance(npgsqlAdapterType, cmd);
                            break;
                    }

                    if (adapter != null)
                    {
                        adapter.Fill(ds);
                        adapter.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("ExecuteDataSet failed: " + sql, ex, _logPath);
                throw;
            }

            return ds;
        }

        /// <summary>
        /// Execute Scalar Query
        /// </summary>
        public object ExecuteScalar(string sql, Dictionary<string, object> parameters = null)
        {
            try
            {
                LogHelper.WriteLog("ExecuteScalar: " + sql, null, _logPath);

                using (DbCommand cmd = CreateCommand(sql, parameters))
                {
                    return cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("ExecuteScalar failed: " + sql, ex, _logPath);
                throw;
            }
        }

        /// <summary>
        /// Execute Non-Query (INSERT, UPDATE, DELETE)
        /// </summary>
        public int ExecuteNonQuery(string sql, Dictionary<string, object> parameters = null)
        {
            try
            {
                LogHelper.WriteLog("ExecuteNonQuery: " + sql, null, _logPath);

                using (DbCommand cmd = CreateCommand(sql, parameters))
                {
                    int result = cmd.ExecuteNonQuery();
                    LogHelper.WriteLog("ExecuteNonQuery affected rows: " + result, null, _logPath);
                    return result;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("ExecuteNonQuery failed: " + sql, ex, _logPath);
                throw;
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Close();
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }
    }
}
