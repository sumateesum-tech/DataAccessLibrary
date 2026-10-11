# DataAccessLibrary - Multi-Database CRUD Library

## ภาพรวม
DataAccessLibrary คือไลบรารี่ .NET Framework สำหรับจัดการฐานข้อมูลหลายชนิด:
- SQL Server
- MySQL
- Oracle
- PostgreSQL

## วิธีใช้งาน

### 1. Setup Connection String
```csharp
DatabaseConfig config = new DatabaseConfig();
config.DatabaseType = DatabaseType.SqlServer;
config.ConnectionString = "Server=localhost;Database=MyDB;User Id=sa;Password=1234;";
config.CommandTimeout = 30;
config.LogPath = Application.StartupPath + "\\logs\\dataaccess.log";
```

### 2. สร้าง IDataAccess Instance
```csharp
IDataAccess db = DataAccessFactory.Create(config);
```

### 3. ใช้ QueryBuilder สำหรับ SELECT
```csharp
var builder = new QueryBuilder(DatabaseType.SqlServer);
var sql = builder
    .Select("u.UserId", "u.UserName", "u.Email", "d.DepartmentName")
    .From("Users", "u")
    .InnerJoin("Departments", "u.DepartmentId = d.DepartmentId", "d")
    .Where("u.Status = @Status", new Dictionary<string, object> { { "@Status", "Active" } })
    .And("d.IsActive = @IsActive", new Dictionary<string, object> { { "@IsActive", true } })
    .OrderBy("u.UserName", "ASC")
    .BuildSelect();

DataTable dt = db.ExecuteQuery(sql, builder.GetParameters());
```

### 4. INSERT
```csharp
var builder = new QueryBuilder(DatabaseType.SqlServer);
var data = new Dictionary<string, object>()
{
    { "UserName", "John Doe" },
    { "Email", "john@example.com" },
    { "Status", "Active" }
};

var sql = builder.BuildInsert("Users", data);
int result = db.ExecuteNonQuery(sql, builder.GetParameters());

if (result > 0)
    MessageBox.Show("Insert successful");
```

### 5. UPDATE
```csharp
var builder = new QueryBuilder(DatabaseType.SqlServer);
var values = new Dictionary<string, object>()
{
    { "UserName", "Jane Doe" },
    { "Email", "jane@example.com" }
};

var whereParams = new Dictionary<string, object>()
{
    { "@UserId", 1 }
};

var sql = builder.BuildUpdate("Users", values, "UserId = @UserId", whereParams);
int result = db.ExecuteNonQuery(sql, builder.GetParameters());
```

### 6. DELETE
```csharp
var builder = new QueryBuilder(DatabaseType.SqlServer);
var whereParams = new Dictionary<string, object>()
{
    { "@UserId", 1 }
};

var sql = builder.BuildDelete("Users", "UserId = @UserId", whereParams);
int result = db.ExecuteNonQuery(sql, builder.GetParameters());
```

## JOIN Examples

### INNER JOIN
```csharp
var builder = new QueryBuilder(DatabaseType.SqlServer);
var sql = builder
    .Select("*")
    .From("Users", "u")
    .InnerJoin("Departments", "u.DepartmentId = d.DepartmentId", "d")
    .BuildSelect();
```

### LEFT JOIN
```csharp
var sql = builder
    .Select("*")
    .From("Users", "u")
    .LeftJoin("Orders", "u.UserId = o.UserId", "o")
    .BuildSelect();
```

### MULTIPLE JOINs
```csharp
var sql = builder
    .Select("u.UserId", "u.UserName", "d.DepartmentName", "o.OrderId")
    .From("Users", "u")
    .InnerJoin("Departments", "u.DepartmentId = d.DepartmentId", "d")
    .LeftJoin("Orders", "u.UserId = o.UserId", "o")
    .Where("u.Status = @Status", new Dictionary<string, object> { { "@Status", "Active" } })
    .OrderBy("u.UserName", "ASC")
    .BuildSelect();
```

## WinForms Example
```csharp
public partial class Form1 : Form
{
    private IDataAccess _db;

    public Form1()
    {
        InitializeComponent();
        
        var config = new DatabaseConfig(
            DatabaseType.SqlServer,
            "Server=localhost;Database=MyDB;User Id=sa;Password=1234;"
        );
        _db = DataAccessFactory.Create(config);
    }

    private void LoadData()
    {
        try
        {
            var builder = new QueryBuilder(DatabaseType.SqlServer);
            var sql = builder
                .Select("*")
                .From("Users")
                .Where("Status = @Status", new Dictionary<string, object> { { "@Status", "Active" } })
                .BuildSelect();

            DataTable dt = _db.ExecuteQuery(sql, builder.GetParameters());
            dataGridView1.DataSource = dt;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error: " + ex.Message);
        }
    }
}
```

## Features
- ✅ QueryBuilder Pattern สำหรับ SQL Query
- ✅ รองรับ SQL Server, MySQL, Oracle, PostgreSQL
- ✅ Automatic parameter binding
- ✅ Logging system
- ✅ Data validation
- ✅ Transaction support
- ✅ Compatible with .NET Framework 3.5+
- ✅ WinForms friendly

## Installation

1. Add reference to DataAccessLibrary.dll
2. Install required NuGet packages based on your database:
   - MySQL: `MySql.Data`
   - Oracle: `Oracle.ManagedDataAccess`
   - PostgreSQL: `Npgsql`

## Log Files
Log files are saved to: `{Application.StartupPath}\logs\dataaccess.log`

## License
MIT License
