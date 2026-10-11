using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLibrary
{
    /// <summary>
    /// Generic QueryBuilder สำหรับสร้าง SQL Query แบบยืดหยุ่น
    /// รองรับ SQL Server, MySQL, Oracle, PostgreSQL
    /// </summary>
    public class QueryBuilder
    {
        private DatabaseType _databaseType;
        private List<string> _selectColumns;
        private List<string> _fromTables;
        private List<string> _joinClauses;
        private List<string> _whereClauses;
        private List<string> _groupByColumns;
        private List<string> _orderByColumns;
        private Dictionary<string, object> _parameters;
        private int _limit;
        private int _offset;

        public QueryBuilder(DatabaseType databaseType = DatabaseType.SqlServer)
        {
            _databaseType = databaseType;
            _selectColumns = new List<string>();
            _fromTables = new List<string>();
            _joinClauses = new List<string>();
            _whereClauses = new List<string>();
            _groupByColumns = new List<string>();
            _orderByColumns = new List<string>();
            _parameters = new Dictionary<string, object>();
            _limit = -1;
            _offset = 0;
        }

        /// <summary>
        /// เพิ่ม SELECT columns
        /// </summary>
        public QueryBuilder Select(params string[] columns)
        {
            if (columns != null && columns.Length > 0)
            {
                _selectColumns.AddRange(columns);
            }
            else
            {
                _selectColumns.Add("*");
            }
            return this;
        }

        /// <summary>
        /// เพิ่ม FROM table
        /// </summary>
        public QueryBuilder From(string tableName, string alias = null)
        {
            string table = string.IsNullOrWhiteSpace(alias) ? tableName : tableName + " AS " + alias;
            _fromTables.Add(table);
            return this;
        }

        /// <summary>
        /// INNER JOIN
        /// </summary>
        public QueryBuilder InnerJoin(string table, string on, string alias = null)
        {
            string joinTable = string.IsNullOrWhiteSpace(alias) ? table : table + " AS " + alias;
            _joinClauses.Add("INNER JOIN " + joinTable + " ON " + on);
            return this;
        }

        /// <summary>
        /// LEFT JOIN
        /// </summary>
        public QueryBuilder LeftJoin(string table, string on, string alias = null)
        {
            string joinTable = string.IsNullOrWhiteSpace(alias) ? table : table + " AS " + alias;
            _joinClauses.Add("LEFT JOIN " + joinTable + " ON " + on);
            return this;
        }

        /// <summary>
        /// RIGHT JOIN
        /// </summary>
        public QueryBuilder RightJoin(string table, string on, string alias = null)
        {
            string joinTable = string.IsNullOrWhiteSpace(alias) ? table : table + " AS " + alias;
            _joinClauses.Add("RIGHT JOIN " + joinTable + " ON " + on);
            return this;
        }

        /// <summary>
        /// FULL OUTER JOIN
        /// </summary>
        public QueryBuilder FullOuterJoin(string table, string on, string alias = null)
        {
            string joinTable = string.IsNullOrWhiteSpace(alias) ? table : table + " AS " + alias;
            _joinClauses.Add("FULL OUTER JOIN " + joinTable + " ON " + on);
            return this;
        }

        /// <summary>
        /// เพิ่ม WHERE clause
        /// </summary>
        public QueryBuilder Where(string condition, Dictionary<string, object> parameters = null)
        {
            _whereClauses.Add("(" + condition + ")");
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    _parameters[param.Key] = param.Value;
                }
            }
            return this;
        }

        /// <summary>
        /// เพิ่ม AND condition
        /// </summary>
        public QueryBuilder And(string condition, Dictionary<string, object> parameters = null)
        {
            _whereClauses.Add("(" + condition + ")");
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    _parameters[param.Key] = param.Value;
                }
            }
            return this;
        }

        /// <summary>
        /// เพิ่ม OR condition
        /// </summary>
        public QueryBuilder Or(string condition, Dictionary<string, object> parameters = null)
        {
            if (_whereClauses.Count > 0)
            {
                _whereClauses.Add("OR (" + condition + ")");
            }
            else
            {
                _whereClauses.Add("(" + condition + ")");
            }
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    _parameters[param.Key] = param.Value;
                }
            }
            return this;
        }

        /// <summary>
        /// GROUP BY
        /// </summary>
        public QueryBuilder GroupBy(params string[] columns)
        {
            if (columns != null && columns.Length > 0)
            {
                _groupByColumns.AddRange(columns);
            }
            return this;
        }

        /// <summary>
        /// ORDER BY
        /// </summary>
        public QueryBuilder OrderBy(string column, string direction = "ASC")
        {
            _orderByColumns.Add(column + " " + direction);
            return this;
        }

        /// <summary>
        /// LIMIT (สำหรับ MySQL, PostgreSQL)
        /// </summary>
        public QueryBuilder Limit(int limit)
        {
            _limit = limit;
            return this;
        }

        /// <summary>
        /// OFFSET
        /// </summary>
        public QueryBuilder Offset(int offset)
        {
            _offset = offset;
            return this;
        }

        /// <summary>
        /// สร้าง SELECT Query
        /// </summary>
        public string BuildSelect()
        {
            StringBuilder sql = new StringBuilder();

            // SELECT
            if (_selectColumns.Count == 0)
                _selectColumns.Add("*");

            sql.Append("SELECT ");
            sql.Append(string.Join(", ", _selectColumns));

            // FROM
            if (_fromTables.Count == 0)
                throw new InvalidOperationException("No table specified in FROM clause.");

            sql.Append(" FROM ");
            sql.Append(string.Join(", ", _fromTables));

            // JOIN
            if (_joinClauses.Count > 0)
            {
                sql.Append(" ");
                sql.Append(string.Join(" ", _joinClauses));
            }

            // WHERE
            if (_whereClauses.Count > 0)
            {
                sql.Append(" WHERE ");
                // รวม WHERE clauses ด้วย AND ถ้าไม่มี OR
                bool hasOr = false;
                foreach (var clause in _whereClauses)
                {
                    if (clause.StartsWith("OR"))
                        hasOr = true;
                }

                if (hasOr)
                {
                    sql.Append(string.Join(" AND ", _whereClauses));
                }
                else
                {
                    sql.Append(string.Join(" AND ", _whereClauses));
                }
            }

            // GROUP BY
            if (_groupByColumns.Count > 0)
            {
                sql.Append(" GROUP BY ");
                sql.Append(string.Join(", ", _groupByColumns));
            }

            // ORDER BY
            if (_orderByColumns.Count > 0)
            {
                sql.Append(" ORDER BY ");
                sql.Append(string.Join(", ", _orderByColumns));
            }

            // LIMIT & OFFSET
            if (_databaseType == DatabaseType.MySql || _databaseType == DatabaseType.PostgreSql)
            {
                if (_limit > 0)
                {
                    sql.Append(" LIMIT " + _limit);
                }
                if (_offset > 0)
                {
                    sql.Append(" OFFSET " + _offset);
                }
            }
            else if (_databaseType == DatabaseType.SqlServer)
            {
                if (_limit > 0 || _offset > 0)
                {
                    if (_orderByColumns.Count == 0)
                        throw new InvalidOperationException("ORDER BY is required for OFFSET/FETCH in SQL Server.");

                    sql.Append(" OFFSET " + _offset + " ROWS");
                    if (_limit > 0)
                        sql.Append(" FETCH NEXT " + _limit + " ROWS ONLY");
                }
            }

            return sql.ToString();
        }

        /// <summary>
        /// สร้าง INSERT Query
        /// </summary>
        public string BuildInsert(string tableName, Dictionary<string, object> values)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("Table name is required.");

            if (values == null || values.Count == 0)
                throw new ArgumentException("Insert values cannot be empty.");

            List<string> columns = new List<string>();
            List<string> paramNames = new List<string>();

            foreach (var item in values)
            {
                columns.Add(item.Key);
                paramNames.Add("@" + item.Key);
                _parameters["@" + item.Key] = item.Value ?? DBNull.Value;
            }

            string sql = "INSERT INTO " + tableName + " (" + string.Join(", ", columns) + ") VALUES (" + string.Join(", ", paramNames) + ")";
            return sql;
        }

        /// <summary>
        /// สร้าง UPDATE Query
        /// </summary>
        public string BuildUpdate(string tableName, Dictionary<string, object> values, string whereClause, Dictionary<string, object> whereParameters = null)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("Table name is required.");

            if (values == null || values.Count == 0)
                throw new ArgumentException("Update values cannot be empty.");

            if (string.IsNullOrWhiteSpace(whereClause))
                throw new ArgumentException("Where clause is required for UPDATE.");

            List<string> setClause = new List<string>();

            foreach (var item in values)
            {
                setClause.Add(item.Key + " = @" + item.Key);
                _parameters["@" + item.Key] = item.Value ?? DBNull.Value;
            }

            if (whereParameters != null)
            {
                foreach (var param in whereParameters)
                {
                    _parameters[param.Key] = param.Value ?? DBNull.Value;
                }
            }

            string sql = "UPDATE " + tableName + " SET " + string.Join(", ", setClause) + " WHERE " + whereClause;
            return sql;
        }

        /// <summary>
        /// สร้าง DELETE Query
        /// </summary>
        public string BuildDelete(string tableName, string whereClause, Dictionary<string, object> whereParameters = null)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("Table name is required.");

            if (string.IsNullOrWhiteSpace(whereClause))
                throw new ArgumentException("Where clause is required for DELETE.");

            if (whereParameters != null)
            {
                foreach (var param in whereParameters)
                {
                    _parameters[param.Key] = param.Value ?? DBNull.Value;
                }
            }

            string sql = "DELETE FROM " + tableName + " WHERE " + whereClause;
            return sql;
        }

        /// <summary>
        /// ได้ Parameters
        /// </summary>
        public Dictionary<string, object> GetParameters()
        {
            return new Dictionary<string, object>(_parameters);
        }

        /// <summary>
        /// ล้าง QueryBuilder
        /// </summary>
        public void Clear()
        {
            _selectColumns.Clear();
            _fromTables.Clear();
            _joinClauses.Clear();
            _whereClauses.Clear();
            _groupByColumns.Clear();
            _orderByColumns.Clear();
            _parameters.Clear();
            _limit = -1;
            _offset = 0;
        }
    }
}
