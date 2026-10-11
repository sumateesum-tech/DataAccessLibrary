# DataAccessLibrary - Complete Implementation Guide

## ภาพรวม
DataAccessLibrary คือไลบรารี .NET Framework ที่ครบครันสำหรับจัดการฐานข้อมูลหลายชนิด พร้อม CRUD, JOIN, Logging และ Validation

## โครงสร้างไฟล์
```
DataAccessLibrary/
├── Core/
│   └── DataAccess.cs              # Core database access class
├── Models/
│   ├── DatabaseType.cs            # Database type enum
│   └── DatabaseConfig.cs          # Configuration class
├── QueryBuilder/
│   └── QueryBuilder.cs            # SQL query builder
├── Validators/
│   └── DataValidator.cs           # Data validation utilities
├── Helpers/
│   └── LogHelper.cs               # Logging utilities
├── Interfaces/
│   └── IDataAccess.cs             # DataAccess interface
├── Factory/
│   └── DataAccessFactory.cs       # Factory pattern for creating instances
├── Repositories/
│   ├── IRepository.cs             # Generic repository interface
│   └── RepositoryBase.cs          # Base repository class for CRUD
├── Examples/
│   ├── Models.cs                  # User & Department model examples
│   ├── CrudHelper.cs              # Simple CRUD helper class
│   ├── DataAccessDemoForm.cs      # WinForms example
│   ├── UsageExample.cs            # Console example
│   └── app.config                 # Configuration example
└── README.md
```

## การติดตั้ง

### 1. ติดตั้ง NuGet Packages (ตามที่ต้องการ)
```
# SQL Server (built-in)
# MySQL
Install-Package MySql.Data

# Oracle
Install-Package Oracle.ManagedDataAccess

# PostgreSQL
Install-Package Npgsql
```

### 2. Copy ไฟล์ทั้งหมด
คัดลอกโฟลเดอร์ DataAccessLibrary ทั้งหมดไปที่โปรเจกต์ของคุณ

### 3. Add Reference
เพิ่ม reference ไปที่ DataAccessLibrary.dll

## การใช้งาน

### วิธีที่ 1: CrudHelper (ง่ายที่สุด)
```csharp
// 1. สร้าง Config
var config = new DatabaseConfig
{
    DatabaseType = DatabaseType.SqlServer,
    ConnectionString = "Server=localhost;Database=DemoDB;User Id=sa;Password=1234;"
};

// 2. สร้าง DataAccess
using (IDataAccess db = DataAccessFactory.Create(config))
{
    db.Open();
    var crud = new CrudHelper(db);

    // SELECT
    var users = crud.Select("Users");
    var activeUsers = crud.Select("Users", 
        "Status = @Status", 
        new Dictionary<string, object> { { "@Status", "Active" } });

    // INSERT
    var newUser = new Dictionary<string, object>
    {
        { "UserName", "John" },
        { "Email", "john@example.com" },
        { "Status", "Active" }
    };
    crud.Insert("Users", newUser);

    // UPDATE
    var updateData = new Dictionary<string, object>
    {
        { "Status", "Inactive" }
    };
    crud.Update("Users", updateData, "UserId = @UserId", 
        new Dictionary<string, object> { { "@UserId", 1 } });

    // DELETE
    crud.Delete("Users", "UserId = @UserId", 
        new Dictionary<string, object> { { "@UserId", 1 } });
}
```

### วิธีที่ 2: Repository Pattern (Recommended)
```csharp
var config = new DatabaseConfig(DatabaseType.SqlServer, 
    "Server=localhost;Database=DemoDB;User Id=sa;Password=1234;");

using (IDataAccess db = DataAccessFactory.Create(config))
{
    db.Open();
    var userRepo = new UserRepository(db);

    // SELECT ALL
    var users = userRepo.GetAll();

    // SELECT BY ID
    var user = userRepo.GetById(1);

    // INSERT
    var newUser = new User 
    { 
        UserName = "John", 
        Email = "john@example.com",
        Status = "Active" 
    };
    userRepo.Add(newUser);

    // UPDATE
    user.UserName = "Jane";
    userRepo.Update(user);

    // DELETE
    userRepo.Delete(1);

    // WITH JOIN
    var usersWithDept = userRepo.GetUsersWithDepartment();
}
```

### วิธีที่ 3: QueryBuilder (Advanced)
```csharp
var builder = new QueryBuilder(DatabaseType.SqlServer);
var sql = builder
    .Select("u.UserId", "u.UserName", "d.DepartmentName")
    .From("Users", "u")
    .InnerJoin("Departments", "u.DepartmentId = d.DepartmentId", "d")
    .Where("u.Status = @Status", 
        new Dictionary<string, object> { { "@Status", "Active" } })
    .OrderBy("u.UserName", "ASC")
    .Limit(10)
    .BuildSelect();

DataTable result = db.ExecuteQuery(sql, builder.GetParameters());
```

## QueryBuilder Methods

### SELECT
```csharp
builder.Select("Column1", "Column2");
builder.Select("*");  // All columns
```

### FROM
```csharp
builder.From("TableName");
builder.From("TableName", "alias");
```

### JOIN
```csharp
builder.InnerJoin("Table", "u.Id = t.UserId", "alias");
builder.LeftJoin("Table", "u.Id = t.UserId", "alias");
builder.RightJoin("Table", "u.Id = t.UserId", "alias");
builder.FullOuterJoin("Table", "u.Id = t.UserId", "alias");
```

### WHERE
```csharp
builder.Where("Column = @Column", new Dictionary<string, object> { { "@Column", value } });
builder.And("Column2 = @Column2", parameters);
builder.Or("Column3 = @Column3", parameters);
```

### GROUP BY & ORDER BY
```csharp
builder.GroupBy("Column1", "Column2");
builder.OrderBy("Column", "ASC");  // or DESC
```

### LIMIT & OFFSET
```csharp
builder.Limit(10);
builder.Offset(20);
```

## CrudHelper Methods

```csharp
// SELECT
crud.Select(tableName);
crud.Select(tableName, whereClause, parameters);
crud.SelectColumns(tableName, columns, whereClause, parameters);

// SELECT WITH JOIN
crud.SelectWithJoin(
    tableName: "Users",
    tableAlias: "u",
    selectColumns: new string[] { "u.UserId", "d.DepartmentName" },
    joinType: "INNER",  // INNER, LEFT, RIGHT, FULL
    joinTable: "Departments",
    joinAlias: "d",
    joinCondition: "u.DepartmentId = d.DepartmentId"
);

// INSERT
crud.Insert(tableName, values);

// UPDATE
crud.Update(tableName, values, whereClause, whereParameters);

// DELETE
crud.Delete(tableName, whereClause, whereParameters);

// COUNT
crud.Count(tableName, whereClause, parameters);

// EXISTS
crud.Exists(tableName, whereClause, parameters);
```

## Database Support

### SQL Server
```csharp
var config = new DatabaseConfig
{
    DatabaseType = DatabaseType.SqlServer,
    ConnectionString = "Server=localhost;Database=DemoDB;User Id=sa;Password=1234;"
};
```

### MySQL
```csharp
var config = new DatabaseConfig
{
    DatabaseType = DatabaseType.MySql,
    ConnectionString = "Server=localhost;Database=DemoDB;User Id=root;Password=1234;"
};
```

### Oracle
```csharp
var config = new DatabaseConfig
{
    DatabaseType = DatabaseType.Oracle,
    ConnectionString = "Data Source=localhost:1521/XE;User Id=system;Password=oracle;"
};
```

### PostgreSQL
```csharp
var config = new DatabaseConfig
{
    DatabaseType = DatabaseType.PostgreSql,
    ConnectionString = "Host=localhost;Port=5432;Database=DemoDB;Username=postgres;Password=1234;"
};
```

## Logging

ทั้ง queries, errors, connection status ถูกบันทึกไป log file อัตโนมัติ:
```
{Application.StartupPath}\logs\dataaccess.log
```

## Error Handling

ทุก operation มี try-catch ที่เขียนไว้แล้ว:
```csharp
try
{
    var data = crud.Select("Users");
}
catch (Exception ex)
{
    MessageBox.Show("Error: " + ex.Message);
}
```

## Data Validation

```csharp
// Check if empty
DataValidator.ThrowIfEmpty(userName, "UserName");

// Check if null
DataValidator.ThrowIfNull(user, "User");

// Validate length
DataValidator.ValidateLength(userName, "UserName", 50);

// Type checking
DataValidator.IsInteger(value);
DataValidator.IsDecimal(value);
DataValidator.IsDate(value);
```

## WinForms Integration

```csharp
public partial class Form1 : Form
{
    private IDataAccess _db;
    private UserRepository _userRepo;

    public Form1()
    {
        InitializeComponent();
        var config = new DatabaseConfig(
            DatabaseType.SqlServer,
            "Server=localhost;Database=DemoDB;User Id=sa;Password=1234;"
        );
        _db = DataAccessFactory.Create(config);
        _userRepo = new UserRepository(_db);
    }

    private void Form_Load(object sender, EventArgs e)
    {
        _db.Open();
        LoadData();
    }

    private void LoadData()
    {
        try
        {
            var users = _userRepo.GetUsersWithDepartment();
            dgvUsers.DataSource = users;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error: " + ex.Message);
        }
    }

    private void Form_FormClosing(object sender, FormClosingEventArgs e)
    {
        _db?.Close();
        _db?.Dispose();
    }
}
```

## Features ที่ครบครัน
- ✅ CRUD Operations (Select, Insert, Update, Delete)
- ✅ QueryBuilder Pattern
- ✅ JOIN Support (INNER, LEFT, RIGHT, FULL OUTER)
- ✅ Repository Pattern
- ✅ Logging System
- ✅ Data Validation
- ✅ Multiple Database Support (SQL Server, MySQL, Oracle, PostgreSQL)
- ✅ .NET Framework 3.5+ Compatible
- ✅ WinForms Friendly
- ✅ Automatic Parameter Binding
- ✅ Exception Handling

## License
MIT License
