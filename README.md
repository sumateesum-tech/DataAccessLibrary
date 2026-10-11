# DataAccessLibrary - Multi-Database Access Layer

## 📋 Overview

DataAccessLibrary เป็นไลบรารี่ .NET Framework ที่ออกแบบมาสำหรับ:
- ✅ WinForms Applications
- ✅ Console Applications  
- ✅ Web Applications
- ✅ Multi-Database Support (SQL Server, MySQL, Oracle, PostgreSQL)
- ✅ .NET Framework 3.5 ขึ้นไป

## 🎯 ลักษณะการใช้งาน

คุณแค่กำหนด 4 สิ่งนี้:
1. **ConnectionString** - เชื่อมต่อ Database
2. **Select** - กรอก table, columns, where clause
3. **Insert** - กรอก table, values
4. **Update/Delete** - กรอก table, values, where clause
5. **Join** - สามารถรวมหลาย table ได้

## 📁 Project Structure

```
DataAccessLibrary/
├── Core/
│   └── DataAccess.cs                    # Main database access class
├── Models/
│   ├── DatabaseType.cs                  # Enum for database types
│   └── DatabaseConfig.cs                # Configuration class
├── QueryBuilder/
│   └── QueryBuilder.cs                  # Fluent SQL query builder
├── Validators/
│   └── DataValidator.cs                 # Data validation utilities
├── Helpers/
│   └── LogHelper.cs                     # Logging utilities
├── Interfaces/
│   └── IDataAccess.cs                   # DataAccess interface
├── Factory/
│   └── DataAccessFactory.cs             # Factory pattern
├── Repositories/
│   ├── IRepository.cs                   # Generic repository interface
│   └── RepositoryBase.cs                # Base repository class
├── Examples/
│   ├── Models.cs                        # User & Department models
│   ├── CrudHelper.cs                    # Simple CRUD helper
│   ├── DataAccessDemoForm.cs            # WinForms example
│   ├── DataAccessDemoForm.Designer.cs   # WinForms designer
│   ├── UsageExample.cs                  # Console examples
│   └── app.config                       # Configuration examples
├── Properties/
│   └── AssemblyInfo.cs                  # Assembly information
├── DataAccessLibrary.csproj             # Project file
├── README.md                            # This file
└── GUIDE.md                             # Complete guide
```

## 🚀 Quick Start

### 1. Create Connection Config
```csharp
var config = new DatabaseConfig
{
    DatabaseType = DatabaseType.SqlServer,
    ConnectionString = "Server=localhost;Database=DemoDB;User Id=sa;Password=1234;",
    LogPath = Application.StartupPath + "\\logs\\dataaccess.log",
    CommandTimeout = 30
};
```

### 2. Create DataAccess Instance
```csharp
using (IDataAccess db = DataAccessFactory.Create(config))
{
    db.Open();
    // Your code here
}
```

### 3. Use CrudHelper for Quick CRUD
```csharp
var crud = new CrudHelper(db);

// SELECT
var users = crud.Select("Users");
var activeUsers = crud.Select("Users", "Status = @Status", 
    new Dictionary<string, object> { { "@Status", "Active" } });

// INSERT
crud.Insert("Users", new Dictionary<string, object>
{
    { "UserName", "John" },
    { "Email", "john@example.com" },
    { "Status", "Active" }
});

// UPDATE
crud.Update("Users", 
    new Dictionary<string, object> { { "Status", "Inactive" } },
    "UserId = @UserId",
    new Dictionary<string, object> { { "@UserId", 1 } });

// DELETE
crud.Delete("Users", "UserId = @UserId",
    new Dictionary<string, object> { { "@UserId", 1 } });
```

### 4. Use Repository Pattern for Type-Safe Operations
```csharp
var userRepo = new UserRepository(db);

// GET ALL
var users = userRepo.GetAll();

// GET BY ID
var user = userRepo.GetById(1);

// ADD
userRepo.Add(new User { UserName = "Jane", Email = "jane@example.com" });

// UPDATE
user.UserName = "Updated";
userRepo.Update(user);

// DELETE
userRepo.Delete(1);
```

### 5. Use QueryBuilder for Complex Queries
```csharp
var builder = new QueryBuilder(DatabaseType.SqlServer);
var sql = builder
    .Select("u.UserId", "u.UserName", "d.DepartmentName")
    .From("Users", "u")
    .InnerJoin("Departments", "u.DepartmentId = d.DepartmentId", "d")
    .Where("u.Status = @Status", 
        new Dictionary<string, object> { { "@Status", "Active" } })
    .OrderBy("u.UserName", "ASC")
    .BuildSelect();

var result = db.ExecuteQuery(sql, builder.GetParameters());
```

## 📚 CrudHelper Methods

### SELECT
```csharp
// Select all
crud.Select("Users");

// Select with WHERE
crud.Select("Users", "Status = @Status", parameters);

// Select specific columns
crud.SelectColumns("Users", new[] { "UserId", "UserName", "Email" });

// Select with JOIN
crud.SelectWithJoin(
    tableName: "Users",
    tableAlias: "u",
    selectColumns: new[] { "u.UserId", "u.UserName", "d.DepartmentName" },
    joinType: "INNER",  // INNER, LEFT, RIGHT, FULL
    joinTable: "Departments",
    joinAlias: "d",
    joinCondition: "u.DepartmentId = d.DepartmentId"
);
```

### INSERT
```csharp
crud.Insert("Users", new Dictionary<string, object>
{
    { "UserName", "John" },
    { "Email", "john@example.com" },
    { "Status", "Active" }
});
```

### UPDATE
```csharp
crud.Update("Users",
    new Dictionary<string, object> { { "UserName", "Jane" } },
    "UserId = @UserId",
    new Dictionary<string, object> { { "@UserId", 1 } });
```

### DELETE
```csharp
crud.Delete("Users", "UserId = @UserId",
    new Dictionary<string, object> { { "@UserId", 1 } });
```

### COUNT & EXISTS
```csharp
int total = crud.Count("Users");
bool exists = crud.Exists("Users", "UserId = @UserId", 
    new Dictionary<string, object> { { "@UserId", 1 } });
```

## 🔗 JOIN Examples

### INNER JOIN
```csharp
var builder = new QueryBuilder(DatabaseType.SqlServer);
var sql = builder
    .Select("u.*", "d.DepartmentName")
    .From("Users", "u")
    .InnerJoin("Departments", "u.DepartmentId = d.DepartmentId", "d")
    .BuildSelect();
```

### LEFT JOIN
```csharp
var sql = builder
    .Select("u.*", "o.OrderId")
    .From("Users", "u")
    .LeftJoin("Orders", "u.UserId = o.UserId", "o")
    .BuildSelect();
```

### RIGHT JOIN
```csharp
var sql = builder
    .Select("u.*", "o.OrderId")
    .From("Users", "u")
    .RightJoin("Orders", "u.UserId = o.UserId", "o")
    .BuildSelect();
```

### FULL OUTER JOIN
```csharp
var sql = builder
    .Select("u.*", "o.OrderId")
    .From("Users", "u")
    .FullOuterJoin("Orders", "u.UserId = o.UserId", "o")
    .BuildSelect();
```

### MULTIPLE JOINs
```csharp
var sql = builder
    .Select("u.UserId", "u.UserName", "d.DepartmentName", "o.OrderId")
    .From("Users", "u")
    .InnerJoin("Departments", "u.DepartmentId = d.DepartmentId", "d")
    .LeftJoin("Orders", "u.UserId = o.UserId", "o")
    .Where("u.Status = @Status", parameters)
    .OrderBy("u.UserName", "ASC")
    .BuildSelect();
```

## 🗄️ Database Support

### SQL Server
```csharp
var config = new DatabaseConfig(
    DatabaseType.SqlServer,
    "Server=localhost;Database=DemoDB;User Id=sa;Password=1234;"
);
```

### MySQL
```csharp
var config = new DatabaseConfig(
    DatabaseType.MySql,
    "Server=localhost;Database=DemoDB;User Id=root;Password=1234;"
);
```

### Oracle
```csharp
var config = new DatabaseConfig(
    DatabaseType.Oracle,
    "Data Source=localhost:1521/XE;User Id=system;Password=oracle;"
);
```

### PostgreSQL
```csharp
var config = new DatabaseConfig(
    DatabaseType.PostgreSql,
    "Host=localhost;Port=5432;Database=DemoDB;Username=postgres;Password=1234;"
);
```

## 📝 Logging

All operations are logged to:
```
{Application.StartupPath}\logs\dataaccess.log
```

Log entries include:
- Timestamp
- SQL Query
- Parameters
- Exception details
- Affected rows count

## ✔️ Validation

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

// Safe string conversion
string safe = DataValidator.SafeString(value);
```

## ⚠️ Error Handling

All operations include try-catch and logging:
```csharp
try
{
    var result = crud.Select("Users");
}
catch (Exception ex)
{
    MessageBox.Show("Error: " + ex.Message);
    // Exception is automatically logged
}
```

## 🎨 WinForms Integration

See `Examples/DataAccessDemoForm.cs` for complete WinForms example with:
- Select with DataGridView
- Insert with TextBox
- Update selected row
- Delete with confirmation
- Search functionality
- JOIN example

## 📦 Installation

1. **Copy Files**: Copy entire DataAccessLibrary folder to your project
2. **Add Reference**: Add DataAccessLibrary.dll to your project references
3. **Install NuGet Packages** (based on your database):
   ```
   Install-Package MySql.Data          # For MySQL
   Install-Package Oracle.ManagedDataAccess  # For Oracle
   Install-Package Npgsql              # For PostgreSQL
   ```
4. **Update Connection String**: Change the connection string in your config
5. **Use in Your Code**: Start using the library

## 💡 Best Practices

1. **Always use `using` statement**
   ```csharp
   using (IDataAccess db = DataAccessFactory.Create(config))
   {
       db.Open();
       // Your code
   }
   ```

2. **Always use parameterized queries**
   ```csharp
   crud.Select("Users", "UserId = @UserId", 
       new Dictionary<string, object> { { "@UserId", id } });
   ```

3. **Use Repository Pattern for type-safe operations**
   ```csharp
   var userRepo = new UserRepository(db);
   var users = userRepo.GetAll();
   ```

4. **Handle exceptions appropriately**
   ```csharp
   try
   {
       // Your code
   }
   catch (Exception ex)
   {
       MessageBox.Show("Error: " + ex.Message);
   }
   ```

## 🔄 Supported Operations

- ✅ SELECT (with WHERE, JOIN, GROUP BY, ORDER BY, LIMIT, OFFSET)
- ✅ INSERT (single and multiple records)
- ✅ UPDATE (with WHERE clause)
- ✅ DELETE (with WHERE clause)
- ✅ COUNT
- ✅ EXISTS
- ✅ INNER JOIN
- ✅ LEFT JOIN
- ✅ RIGHT JOIN
- ✅ FULL OUTER JOIN
- ✅ Multiple JOINs
- ✅ Transactions
- ✅ Parameterized queries
- ✅ Logging
- ✅ Validation

## 📞 Support

For issues or questions:
1. Check `GUIDE.md` for detailed documentation
2. See `Examples/UsageExample.cs` for code samples
3. Review `Examples/DataAccessDemoForm.cs` for WinForms implementation

## 📄 License

MIT License - Free for personal and commercial use

## 🎉 Features

- Multi-Database Support (SQL Server, MySQL, Oracle, PostgreSQL)
- Easy to use API (CrudHelper, Repository Pattern, QueryBuilder)
- Automatic parameter binding
- Comprehensive logging
- Data validation
- WinForms ready
- .NET Framework 3.5+ compatible
- No external dependencies required (uses built-in ADO.NET)
- Exception handling
- Connection management
- Type-safe operations

---

**Version**: 1.0.0  
**Author**: DataAccessLibrary Team  
**Last Updated**: 2024  
