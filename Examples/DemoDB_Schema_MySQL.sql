-- ============================================
-- MySQL Demo Database Schema
-- ============================================

CREATE DATABASE IF NOT EXISTS DemoDB;
USE DemoDB;

-- Create Departments Table
CREATE TABLE IF NOT EXISTS Departments (
    DepartmentId INT AUTO_INCREMENT PRIMARY KEY,
    DepartmentName VARCHAR(100) NOT NULL,
    Description VARCHAR(255),
    IsActive TINYINT DEFAULT 1
);

-- Create Users Table
CREATE TABLE IF NOT EXISTS Users (
    UserId INT AUTO_INCREMENT PRIMARY KEY,
    UserName VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    PhoneNumber VARCHAR(20),
    Status VARCHAR(20),
    DepartmentId INT NOT NULL,
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (DepartmentId) REFERENCES Departments(DepartmentId)
);

-- Insert Sample Departments
INSERT INTO Departments (DepartmentName, Description, IsActive) VALUES
('IT', 'Information Technology', 1),
('HR', 'Human Resources', 1),
('Sales', 'Sales Department', 1),
('Finance', 'Finance Department', 1);

-- Insert Sample Users
INSERT INTO Users (UserName, Email, PhoneNumber, Status, DepartmentId) VALUES
('John Doe', 'john@example.com', '123-456-7890', 'Active', 1),
('Jane Smith', 'jane@example.com', '123-456-7891', 'Active', 1),
('Bob Johnson', 'bob@example.com', '123-456-7892', 'Active', 2),
('Alice Williams', 'alice@example.com', '123-456-7893', 'Inactive', 3),
('Charlie Brown', 'charlie@example.com', '123-456-7894', 'Active', 4);

-- Verify
SELECT * FROM Departments;
SELECT * FROM Users;
