using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DataAccessLibrary.Examples
{
    /// <summary>
    /// ตัวอย่าง Model สำหรับ User Table
    /// </summary>
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Status { get; set; }
        public int DepartmentId { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    /// <summary>
    /// User Repository สำหรับจัดการ CRUD ของ User
    /// </summary>
    public class UserRepository : RepositoryBase<User>
    {
        public UserRepository(IDataAccess dataAccess)
            : base(dataAccess, "Users", "UserId")
        {
        }

        /// <summary>
        /// ดึงข้อมูล User พร้อมชื่อ Department
        /// </summary>
        public DataTable GetUsersWithDepartment()
        {
            try
            {
                var builder = new QueryBuilder(_dataAccess.DatabaseType);
                var sql = builder
                    .Select("u.UserId", "u.UserName", "u.Email", "u.PhoneNumber", "u.Status", "d.DepartmentName", "u.CreatedDate")
                    .From("Users", "u")
                    .InnerJoin("Departments", "u.DepartmentId = d.DepartmentId", "d")
                    .OrderBy("u.UserName", "ASC")
                    .BuildSelect();

                return _dataAccess.ExecuteQuery(sql, builder.GetParameters());
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("GetUsersWithDepartment error", ex);
                throw;
            }
        }

        /// <summary>
        /// ดึงข้อมูล User ที่ Active เท่านั้น
        /// </summary>
        public DataTable GetActiveUsers()
        {
            try
            {
                var builder = new QueryBuilder(_dataAccess.DatabaseType);
                var sql = builder
                    .Select("*")
                    .From(_tableName)
                    .Where("Status = @Status", new Dictionary<string, object> { { "@Status", "Active" } })
                    .OrderBy("UserName", "ASC")
                    .BuildSelect();

                return _dataAccess.ExecuteQuery(sql, builder.GetParameters());
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("GetActiveUsers error", ex);
                throw;
            }
        }

        /// <summary>
        /// ค้นหา User ตามชื่อ
        /// </summary>
        public DataTable SearchByName(string userName)
        {
            try
            {
                DataValidator.ThrowIfEmpty(userName, "UserName");

                var builder = new QueryBuilder(_dataAccess.DatabaseType);
                var sql = builder
                    .Select("*")
                    .From(_tableName)
                    .Where("UserName LIKE @UserName", new Dictionary<string, object> { { "@UserName", "%" + userName + "%" } })
                    .OrderBy("UserName", "ASC")
                    .BuildSelect();

                return _dataAccess.ExecuteQuery(sql, builder.GetParameters());
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("SearchByName error", ex);
                throw;
            }
        }
    }

    /// <summary>
    /// ตัวอย่าง Model สำหรับ Department Table
    /// </summary>
    public class Department
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Department Repository
    /// </summary>
    public class DepartmentRepository : RepositoryBase<Department>
    {
        public DepartmentRepository(IDataAccess dataAccess)
            : base(dataAccess, "Departments", "DepartmentId")
        {
        }

        /// <summary>
        /// ดึง Department ที่ Active เท่านั้น
        /// </summary>
        public DataTable GetActiveDepartments()
        {
            try
            {
                var builder = new QueryBuilder(_dataAccess.DatabaseType);
                var sql = builder
                    .Select("*")
                    .From(_tableName)
                    .Where("IsActive = @IsActive", new Dictionary<string, object> { { "@IsActive", true } })
                    .OrderBy("DepartmentName", "ASC")
                    .BuildSelect();

                return _dataAccess.ExecuteQuery(sql, builder.GetParameters());
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("GetActiveDepartments error", ex);
                throw;
            }
        }
    }
}
