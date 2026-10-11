-- ============================================
-- Demo Database Schema
-- SQL Server Script
-- ============================================

-- Create Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'DemoDB')
BEGIN
    CREATE DATABASE DemoDB
END
GO

USE DemoDB
GO

-- Create Departments Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Departments')
BEGIN
    CREATE TABLE Departments (
        DepartmentId INT PRIMARY KEY IDENTITY(1,1),
        DepartmentName NVARCHAR(100) NOT NULL,
        Description NVARCHAR(255),
        IsActive BIT DEFAULT 1
    )
END
GO

-- Create Users Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        UserId INT PRIMARY KEY IDENTITY(1,1),
        UserName NVARCHAR(100) NOT NULL,
        Email NVARCHAR(100) NOT NULL,
        PhoneNumber NVARCHAR(20),
        Status NVARCHAR(20),
        DepartmentId INT NOT NULL,
        CreatedDate DATETIME DEFAULT GETDATE(),
        FOREIGN KEY (DepartmentId) REFERENCES Departments(DepartmentId)
    )
END
GO

-- Insert Sample Departments
IF NOT EXISTS (SELECT * FROM Departments WHERE DepartmentName = 'IT')
BEGIN
    INSERT INTO Departments (DepartmentName, Description, IsActive) 
    VALUES 
        ('IT', 'Information Technology', 1),
        ('HR', 'Human Resources', 1),
        ('Sales', 'Sales Department', 1),
        ('Finance', 'Finance Department', 1)
END
GO

-- Insert Sample Users
IF NOT EXISTS (SELECT * FROM Users WHERE UserName = 'John Doe')
BEGIN
    INSERT INTO Users (UserName, Email, PhoneNumber, Status, DepartmentId, CreatedDate)
    VALUES
        ('John Doe', 'john@example.com', '123-456-7890', 'Active', 1, GETDATE()),
        ('Jane Smith', 'jane@example.com', '123-456-7891', 'Active', 1, GETDATE()),
        ('Bob Johnson', 'bob@example.com', '123-456-7892', 'Active', 2, GETDATE()),
        ('Alice Williams', 'alice@example.com', '123-456-7893', 'Inactive', 3, GETDATE()),
        ('Charlie Brown', 'charlie@example.com', '123-456-7894', 'Active', 4, GETDATE())
END
GO

-- Verify tables
SELECT * FROM Departments
SELECT * FROM Users
GO
