using System;
using System.Collections.Generic;
using System.Data;

namespace DataAccessLibrary
{
    public interface IDataAccess : IDisposable
    {
        string ConnectionString { get; set; }
        DatabaseType DatabaseType { get; }

        void Open();
        void Close();

        DataTable ExecuteQuery(string sql, Dictionary<string, object> parameters = null);
        DataSet ExecuteDataSet(string sql, Dictionary<string, object> parameters = null);
        object ExecuteScalar(string sql, Dictionary<string, object> parameters = null);
        int ExecuteNonQuery(string sql, Dictionary<string, object> parameters = null);
    }
}
