using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace DataAccessLibrary
{
    /// <summary>
    /// Generic Repository สำหรับจัดการ CRUD operations แบบอัตโนมัติ
    /// </summary>
    public abstract class RepositoryBase<T> : IRepository<T> where T : class, new()
    {
        protected IDataAccess _dataAccess;
        protected string _tableName;
        protected string _primaryKeyName;

        public RepositoryBase(IDataAccess dataAccess, string tableName, string primaryKeyName = "Id")
        {
            _dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
            _tableName = tableName ?? throw new ArgumentNullException(nameof(tableName));
            _primaryKeyName = primaryKeyName;
        }

        /// <summary>
        /// ดึงข้อมูลทั้งหมด
        /// </summary>
        public virtual IEnumerable<T> GetAll()
        {
            try
            {
                var builder = new QueryBuilder(_dataAccess.DatabaseType);
                var sql = builder.Select("*").From(_tableName).BuildSelect();
                DataTable dt = _dataAccess.ExecuteQuery(sql);
                return ConvertDataTableToList(dt);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("GetAll error", ex);
                throw;
            }
        }

        /// <summary>
        /// ดึงข้อมูลตาม ID
        /// </summary>
        public virtual T GetById(object id)
        {
            try
            {
                DataValidator.ThrowIfNull(id, _primaryKeyName);

                var builder = new QueryBuilder(_dataAccess.DatabaseType);
                var whereParams = new Dictionary<string, object> { { "@" + _primaryKeyName, id } };
                var sql = builder.Select("*")
                    .From(_tableName)
                    .Where(_primaryKeyName + " = @" + _primaryKeyName, whereParams)
                    .BuildSelect();

                DataTable dt = _dataAccess.ExecuteQuery(sql, builder.GetParameters());
                var result = ConvertDataTableToList(dt);
                return result.Count > 0 ? result[0] : null;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("GetById error", ex);
                throw;
            }
        }

        /// <summary>
        /// เพิ่มข้อมูลใหม่
        /// </summary>
        public virtual int Add(T entity)
        {
            try
            {
                DataValidator.ThrowIfNull(entity, "entity");

                var values = ConvertObjectToDictionary(entity);
                if (values.ContainsKey(_primaryKeyName))
                    values.Remove(_primaryKeyName);

                var builder = new QueryBuilder(_dataAccess.DatabaseType);
                var sql = builder.BuildInsert(_tableName, values);
                return _dataAccess.ExecuteNonQuery(sql, builder.GetParameters());
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Add error", ex);
                throw;
            }
        }

        /// <summary>
        /// อัปเดตข้อมูล
        /// </summary>
        public virtual int Update(T entity)
        {
            try
            {
                DataValidator.ThrowIfNull(entity, "entity");

                var values = ConvertObjectToDictionary(entity);
                object idValue = values.ContainsKey(_primaryKeyName) ? values[_primaryKeyName] : null;

                if (idValue == null)
                    throw new ArgumentException(_primaryKeyName + " is required for update.");

                values.Remove(_primaryKeyName);

                var whereParams = new Dictionary<string, object> { { "@" + _primaryKeyName, idValue } };
                var builder = new QueryBuilder(_dataAccess.DatabaseType);
                var sql = builder.BuildUpdate(_tableName, values, _primaryKeyName + " = @" + _primaryKeyName, whereParams);
                return _dataAccess.ExecuteNonQuery(sql, builder.GetParameters());
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Update error", ex);
                throw;
            }
        }

        /// <summary>
        /// ลบข้อมูล
        /// </summary>
        public virtual int Delete(object id)
        {
            try
            {
                DataValidator.ThrowIfNull(id, _primaryKeyName);

                var whereParams = new Dictionary<string, object> { { "@" + _primaryKeyName, id } };
                var builder = new QueryBuilder(_dataAccess.DatabaseType);
                var sql = builder.BuildDelete(_tableName, _primaryKeyName + " = @" + _primaryKeyName, whereParams);
                return _dataAccess.ExecuteNonQuery(sql, builder.GetParameters());
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Delete error", ex);
                throw;
            }
        }

        /// <summary>
        /// ตรวจสอบว่ามีข้อมูลหรือไม่
        /// </summary>
        public virtual bool Exists(object id)
        {
            try
            {
                DataValidator.ThrowIfNull(id, _primaryKeyName);

                var whereParams = new Dictionary<string, object> { { "@" + _primaryKeyName, id } };
                var builder = new QueryBuilder(_dataAccess.DatabaseType);
                var sql = builder.Select("COUNT(*) as Count")
                    .From(_tableName)
                    .Where(_primaryKeyName + " = @" + _primaryKeyName, whereParams)
                    .BuildSelect();

                object result = _dataAccess.ExecuteScalar(sql, builder.GetParameters());
                return result != null && Convert.ToInt32(result) > 0;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Exists error", ex);
                return false;
            }
        }

        /// <summary>
        /// แปลง DataTable เป็น List<T>
        /// </summary>
        protected List<T> ConvertDataTableToList(DataTable table)
        {
            List<T> list = new List<T>();

            if (table == null || table.Rows.Count == 0)
                return list;

            PropertyInfo[] properties = typeof(T).GetProperties();

            foreach (DataRow row in table.Rows)
            {
                T item = new T();
                foreach (PropertyInfo prop in properties)
                {
                    if (table.Columns.Contains(prop.Name))
                    {
                        object value = row[prop.Name];
                        if (value != DBNull.Value)
                        {
                            prop.SetValue(item, Convert.ChangeType(value, prop.PropertyType), null);
                        }
                    }
                }
                list.Add(item);
            }

            return list;
        }

        /// <summary>
        /// แปลง Object เป็น Dictionary
        /// </summary>
        protected Dictionary<string, object> ConvertObjectToDictionary(T entity)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            PropertyInfo[] properties = typeof(T).GetProperties();

            foreach (PropertyInfo prop in properties)
            {
                if (prop.CanRead)
                {
                    object value = prop.GetValue(entity, null);
                    dict[prop.Name] = value ?? DBNull.Value;
                }
            }

            return dict;
        }
    }
}
