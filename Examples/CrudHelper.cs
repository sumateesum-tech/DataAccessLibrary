using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.Examples
{
    /// <summary>
    /// คลาส Utility สำหรับ CRUD operations ที่ใช้งานง่ายๆ
    /// </summary>
    public class CrudHelper
    {
        private IDataAccess _dataAccess;
        private DatabaseType _databaseType;

        public CrudHelper(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
            _databaseType = dataAccess.DatabaseType;
        }

        /// <summary>
        /// SELECT ข้อมูล - กรอก table และ where clause
        /// </summary>
        public System.Data.DataTable Select(string tableName, string whereClause = null, Dictionary<string, object> parameters = null)
        {
            try
            {
                DataValidator.ThrowIfEmpty(tableName, "Table Name");

                var builder = new QueryBuilder(_databaseType);
                builder.Select("*").From(tableName);

                if (!string.IsNullOrWhiteSpace(whereClause))
                {
                    builder.Where(whereClause, parameters);
                }

                string sql = builder.BuildSelect();
                return _dataAccess.ExecuteQuery(sql, builder.GetParameters());
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Select error", ex);
                throw;
            }
        }

        /// <summary>
        /// SELECT ข้อมูล - กรอก columns, table และ where clause
        /// </summary>
        public System.Data.DataTable SelectColumns(string tableName, string[] columns, string whereClause = null, Dictionary<string, object> parameters = null)
        {
            try
            {
                DataValidator.ThrowIfEmpty(tableName, "Table Name");

                var builder = new QueryBuilder(_databaseType);
                if (columns != null && columns.Length > 0)
                    builder.Select(columns);
                else
                    builder.Select("*");

                builder.From(tableName);

                if (!string.IsNullOrWhiteSpace(whereClause))
                {
                    builder.Where(whereClause, parameters);
                }

                string sql = builder.BuildSelect();
                return _dataAccess.ExecuteQuery(sql, builder.GetParameters());
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("SelectColumns error", ex);
                throw;
            }
        }

        /// <summary>
        /// SELECT ข้อมูลพร้อม JOIN
        /// </summary>
        public System.Data.DataTable SelectWithJoin(string tableName, string tableAlias, string[] selectColumns,
            string joinType, string joinTable, string joinAlias, string joinCondition,
            string whereClause = null, Dictionary<string, object> parameters = null)
        {
            try
            {
                DataValidator.ThrowIfEmpty(tableName, "Table Name");
                DataValidator.ThrowIfEmpty(joinTable, "Join Table");
                DataValidator.ThrowIfEmpty(joinCondition, "Join Condition");

                var builder = new QueryBuilder(_databaseType);

                if (selectColumns != null && selectColumns.Length > 0)
                    builder.Select(selectColumns);
                else
                    builder.Select("*");

                builder.From(tableName, tableAlias);

                switch (joinType.ToUpper())
                {
                    case "INNER":
                        builder.InnerJoin(joinTable, joinCondition, joinAlias);
                        break;
                    case "LEFT":
                        builder.LeftJoin(joinTable, joinCondition, joinAlias);
                        break;
                    case "RIGHT":
                        builder.RightJoin(joinTable, joinCondition, joinAlias);
                        break;
                    case "FULL":
                        builder.FullOuterJoin(joinTable, joinCondition, joinAlias);
                        break;
                    default:
                        throw new ArgumentException("Invalid join type. Use: INNER, LEFT, RIGHT, or FULL");
                }

                if (!string.IsNullOrWhiteSpace(whereClause))
                {
                    builder.Where(whereClause, parameters);
                }

                string sql = builder.BuildSelect();
                return _dataAccess.ExecuteQuery(sql, builder.GetParameters());
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("SelectWithJoin error", ex);
                throw;
            }
        }

        /// <summary>
        /// INSERT ข้อมูล - กรอก table และ values
        /// </summary>
        public int Insert(string tableName, Dictionary<string, object> values)
        {
            try
            {
                DataValidator.ThrowIfEmpty(tableName, "Table Name");
                if (values == null || values.Count == 0)
                    throw new ArgumentException("Values cannot be empty");

                var builder = new QueryBuilder(_databaseType);
                string sql = builder.BuildInsert(tableName, values);
                return _dataAccess.ExecuteNonQuery(sql, builder.GetParameters());
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Insert error", ex);
                throw;
            }
        }

        /// <summary>
        /// UPDATE ข้อมูล - กรอก table, values และ where clause
        /// </summary>
        public int Update(string tableName, Dictionary<string, object> values, string whereClause, Dictionary<string, object> whereParameters = null)
        {
            try
            {
                DataValidator.ThrowIfEmpty(tableName, "Table Name");
                if (values == null || values.Count == 0)
                    throw new ArgumentException("Values cannot be empty");
                DataValidator.ThrowIfEmpty(whereClause, "Where Clause");

                var builder = new QueryBuilder(_databaseType);
                string sql = builder.BuildUpdate(tableName, values, whereClause, whereParameters);
                return _dataAccess.ExecuteNonQuery(sql, builder.GetParameters());
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Update error", ex);
                throw;
            }
        }

        /// <summary>
        /// DELETE ข้อมูล - กรอก table และ where clause
        /// </summary>
        public int Delete(string tableName, string whereClause, Dictionary<string, object> whereParameters = null)
        {
            try
            {
                DataValidator.ThrowIfEmpty(tableName, "Table Name");
                DataValidator.ThrowIfEmpty(whereClause, "Where Clause");

                var builder = new QueryBuilder(_databaseType);
                string sql = builder.BuildDelete(tableName, whereClause, whereParameters);
                return _dataAccess.ExecuteNonQuery(sql, builder.GetParameters());
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Delete error", ex);
                throw;
            }
        }

        /// <summary>
        /// COUNT ข้อมูล
        /// </summary>
        public int Count(string tableName, string whereClause = null, Dictionary<string, object> parameters = null)
        {
            try
            {
                DataValidator.ThrowIfEmpty(tableName, "Table Name");

                var builder = new QueryBuilder(_databaseType);
                builder.Select("COUNT(*) as Count").From(tableName);

                if (!string.IsNullOrWhiteSpace(whereClause))
                {
                    builder.Where(whereClause, parameters);
                }

                string sql = builder.BuildSelect();
                object result = _dataAccess.ExecuteScalar(sql, builder.GetParameters());
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Count error", ex);
                throw;
            }
        }

        /// <summary>
        /// ตรวจสอบว่ามีข้อมูลหรือไม่
        /// </summary>
        public bool Exists(string tableName, string whereClause, Dictionary<string, object> parameters = null)
        {
            try
            {
                return Count(tableName, whereClause, parameters) > 0;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Exists error", ex);
                return false;
            }
        }
    }
}
