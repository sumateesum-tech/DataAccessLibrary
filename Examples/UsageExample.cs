using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DataAccessLibrary;
using DataAccessLibrary.Examples;

namespace DataAccessLibrary.Demo
{
    /// <summary>
    /// ตัวอย่างการใช้ DataAccessLibrary แบบ Simple
    /// </summary>
    public class UsageExample
    {
        public static void Main()
        {
            // ============================================
            // ตัวอย่างที่ 1: ใช้ CrudHelper (ง่ายที่สุด)
            // ============================================
            ExampleUseCrudHelper();

            // ============================================
            // ตัวอย่างที่ 2: ใช้ Repository Pattern
            // ============================================
            // ExampleUseRepository();

            // ============================================
            // ตัวอย่างที่ 3: ใช้ QueryBuilder (Advanced)
            // ============================================
            // ExampleUseQueryBuilder();
        }

        // ============================================
        // ตัวอย่างที่ 1: CrudHelper (ง่ายที่สุด)
        // ============================================
        private static void ExampleUseCrudHelper()
        {
            try
            {
                // 1. สร้าง Config
                var config = new DatabaseConfig
                {
                    DatabaseType = DatabaseType.SqlServer,
                    ConnectionString = "Server=localhost;Database=DemoDB;User Id=sa;Password=1234;",
                    LogPath = AppDomain.CurrentDomain.BaseDirectory + "\\logs\\dataaccess.log"
                };

                // 2. สร้าง DataAccess
                using (IDataAccess db = DataAccessFactory.Create(config))
                {
                    db.Open();

                    // 3. สร้าง CrudHelper
                    var crud = new CrudHelper(db);

                    // ============ SELECT ============
                    Console.WriteLine("\n===== SELECT ALL =====");
                    var allUsers = crud.Select("Users");
                    PrintDataTable(allUsers);

                    // ============ SELECT WITH WHERE ============
                    Console.WriteLine("\n===== SELECT WITH WHERE =====");
                    var activeUsers = crud.Select("Users",
                        "Status = @Status",
                        new Dictionary<string, object> { { "@Status", "Active" } });
                    PrintDataTable(activeUsers);

                    // ============ SELECT SPECIFIC COLUMNS ============
                    Console.WriteLine("\n===== SELECT SPECIFIC COLUMNS =====");
                    var userColumns = crud.SelectColumns("Users",
                        new string[] { "UserId", "UserName", "Email" });
                    PrintDataTable(userColumns);

                    // ============ SELECT WITH JOIN ============
                    Console.WriteLine("\n===== SELECT WITH JOIN =====");
                    var usersWithDept = crud.SelectWithJoin(
                        tableName: "Users",
                        tableAlias: "u",
                        selectColumns: new string[] { "u.UserId", "u.UserName", "u.Email", "d.DepartmentName" },
                        joinType: "INNER",
                        joinTable: "Departments",
                        joinAlias: "d",
                        joinCondition: "u.DepartmentId = d.DepartmentId"
                    );
                    PrintDataTable(usersWithDept);

                    // ============ INSERT ============
                    Console.WriteLine("\n===== INSERT =====");
                    var newUser = new Dictionary<string, object>
                    {
                        { "UserName", "John Doe" },
                        { "Email", "john@example.com" },
                        { "PhoneNumber", "123-456-7890" },
                        { "Status", "Active" },
                        { "DepartmentId", 1 },
                        { "CreatedDate", DateTime.Now }
                    };
                    int insertResult = crud.Insert("Users", newUser);
                    Console.WriteLine("Rows inserted: " + insertResult);

                    // ============ UPDATE ============
                    Console.WriteLine("\n===== UPDATE =====");
                    var updateValues = new Dictionary<string, object>
                    {
                        { "UserName", "Jane Doe" },
                        { "Email", "jane@example.com" }
                    };
                    int updateResult = crud.Update("Users",
                        updateValues,
                        "UserId = @UserId",
                        new Dictionary<string, object> { { "@UserId", 1 } });
                    Console.WriteLine("Rows updated: " + updateResult);

                    // ============ DELETE ============
                    Console.WriteLine("\n===== DELETE =====");
                    int deleteResult = crud.Delete("Users",
                        "UserId = @UserId",
                        new Dictionary<string, object> { { "@UserId", 1 } });
                    Console.WriteLine("Rows deleted: " + deleteResult);

                    // ============ COUNT ============
                    Console.WriteLine("\n===== COUNT =====");
                    int totalUsers = crud.Count("Users");
                    Console.WriteLine("Total users: " + totalUsers);

                    // ============ EXISTS ============
                    Console.WriteLine("\n===== EXISTS =====");
                    bool exists = crud.Exists("Users",
                        "UserId = @UserId",
                        new Dictionary<string, object> { { "@UserId", 1 } });
                    Console.WriteLine("User exists: " + exists);
                }

                Console.WriteLine("\n\nPress any key to exit...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.ReadKey();
            }
        }

        // ============================================
        // ตัวอย่างที่ 2: Repository Pattern
        // ============================================
        private static void ExampleUseRepository()
        {
            try
            {
                var config = new DatabaseConfig
                {
                    DatabaseType = DatabaseType.SqlServer,
                    ConnectionString = "Server=localhost;Database=DemoDB;User Id=sa;Password=1234;"
                };

                using (IDataAccess db = DataAccessFactory.Create(config))
                {
                    db.Open();

                    // สร้าง UserRepository
                    var userRepo = new UserRepository(db);

                    // ============ GET ALL ============
                    var allUsers = userRepo.GetAll();
                    foreach (var user in allUsers)
                    {
                        Console.WriteLine($"User: {user.UserName} - {user.Email}");
                    }

                    // ============ GET BY ID ============
                    var user1 = userRepo.GetById(1);
                    if (user1 != null)
                    {
                        Console.WriteLine($"Found: {user1.UserName}");
                    }

                    // ============ ADD ============
                    var newUser = new User
                    {
                        UserName = "New User",
                        Email = "new@example.com",
                        Status = "Active",
                        DepartmentId = 1,
                        CreatedDate = DateTime.Now
                    };
                    int result = userRepo.Add(newUser);
                    Console.WriteLine($"Added: {result} rows");

                    // ============ UPDATE ============
                    if (user1 != null)
                    {
                        user1.UserName = "Updated Name";
                        int updateResult = userRepo.Update(user1);
                        Console.WriteLine($"Updated: {updateResult} rows");
                    }

                    // ============ DELETE ============
                    int deleteResult = userRepo.Delete(1);
                    Console.WriteLine($"Deleted: {deleteResult} rows");

                    // ============ GET WITH JOIN ============
                    var usersWithDept = userRepo.GetUsersWithDepartment();
                    PrintDataTable(usersWithDept);

                    // ============ SEARCH ============
                    var searchResults = userRepo.SearchByName("John");
                    PrintDataTable(searchResults);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        // ============================================
        // ตัวอย่างที่ 3: QueryBuilder (Advanced)
        // ============================================
        private static void ExampleUseQueryBuilder()
        {
            try
            {
                var config = new DatabaseConfig
                {
                    DatabaseType = DatabaseType.SqlServer,
                    ConnectionString = "Server=localhost;Database=DemoDB;User Id=sa;Password=1234;"
                };

                using (IDataAccess db = DataAccessFactory.Create(config))
                {
                    db.Open();

                    // ============ COMPLEX SELECT ============
                    var builder = new QueryBuilder(DatabaseType.SqlServer);
                    string sql = builder
                        .Select("u.UserId", "u.UserName", "u.Email", "d.DepartmentName", "COUNT(*) as OrderCount")
                        .From("Users", "u")
                        .InnerJoin("Departments", "u.DepartmentId = d.DepartmentId", "d")
                        .LeftJoin("Orders", "u.UserId = o.UserId", "o")
                        .Where("u.Status = @Status", new Dictionary<string, object> { { "@Status", "Active" } })
                        .And("d.IsActive = @IsActive", new Dictionary<string, object> { { "@IsActive", true } })
                        .GroupBy("u.UserId", "u.UserName", "u.Email", "d.DepartmentName")
                        .OrderBy("u.UserName", "ASC")
                        .Limit(10)
                        .BuildSelect();

                    Console.WriteLine("Generated SQL:");
                    Console.WriteLine(sql);
                    Console.WriteLine();

                    var result = db.ExecuteQuery(sql, builder.GetParameters());
                    PrintDataTable(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        // Helper method to print DataTable
        private static void PrintDataTable(System.Data.DataTable table)
        {
            if (table == null || table.Rows.Count == 0)
            {
                Console.WriteLine("No data found.");
                return;
            }

            foreach (System.Data.DataColumn column in table.Columns)
            {
                Console.Write(column.ColumnName + "\t");
            }
            Console.WriteLine();

            foreach (System.Data.DataRow row in table.Rows)
            {
                foreach (var item in row.ItemArray)
                {
                    Console.Write((item ?? "NULL") + "\t");
                }
                Console.WriteLine();
            }
        }
    }
}
