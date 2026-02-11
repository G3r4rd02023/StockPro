-- Script adaptado para SQL Server (nombres en PascalCase según EF Core por defecto)
-- Inserciones de ejemplo para la base de datos LocalDB / SQL Server
-- Ejecute este script en la base de datos StockProDb (por ejemplo usando SQL Server Management Studio o sqlcmd)

SET NOCOUNT ON;

BEGIN TRANSACTION;

-- Insertar usuarios por defecto (Role: Admin = 1, Employee = 0)
INSERT INTO Users (Email, PasswordHash, FullName, Role, CreatedAt, UpdatedAt)
VALUES
('admin@inventario.com', '$2b$10$XxXxXxXxXxXxXxXxXxXxXxXxXxXxXxXxXxXxXxXxXxXxXxXx', 'Administrador', 1,SYSUTCDATETIME(),
    SYSUTCDATETIME()),
('empleado@inventario.com', '$2b$10$YyYyYyYyYyYyYyYyYyYyYyYyYyYyYyYyYyYyYyYyYyYyYyYy', 'Empleado General', 0,SYSUTCDATETIME(),
    SYSUTCDATETIME());

-- Insertar categorías de ejemplo
INSERT INTO Categories (Name, ColorHex, CreatedAt, UpdatedAt)
VALUES
('Electrónica', '#3B82F6',SYSUTCDATETIME(),SYSUTCDATETIME()),
('Alimentos', '#10B981',SYSUTCDATETIME(),SYSUTCDATETIME()),
('Ropa', '#8B5CF6',SYSUTCDATETIME(),SYSUTCDATETIME()),
('Hogar', '#F59E0B',SYSUTCDATETIME(),SYSUTCDATETIME()),
('Libros', '#EF4444',SYSUTCDATETIME(),SYSUTCDATETIME());

-- Insertar productos de ejemplo usando el Id de la categoría (SELECT TOP 1 para asegurar un solo valor)
INSERT INTO Products (Name, SKU, CategoryId, Price, CurrentStock, MinStockThreshold, CreatedAt, UpdatedAt)
VALUES (
    'Laptop Dell XPS 13',
    'ELEC-001',
    (SELECT TOP 1 Id FROM Categories WHERE Name = 'Electrónica'),
    1299.99,
    15,
    5,
    SYSUTCDATETIME(),
    SYSUTCDATETIME()
);

INSERT INTO Products (Name, SKU, CategoryId, Price, CurrentStock, MinStockThreshold, CreatedAt, UpdatedAt)
VALUES (
    'Arroz Integral 1kg',
    'ALIM-001',
    (SELECT TOP 1 Id FROM Categories WHERE Name = 'Alimentos'),
    3.50,
    100,
    20,
    SYSUTCDATETIME(),
    SYSUTCDATETIME()
);

COMMIT TRANSACTION;

-- Nota:
-- - Asegúrate de que las tablas "Users", "Categories" y "Products" existen y sus columnas coinciden con los nombres usados aquí.
-- - Si tus tablas usan un esquema distinto o nombres en minúsculas, ajusta los identificadores.
-- - Este script no establece explícitamente valores para las columnas Id si la tabla tiene DEFAULT NEWID().

SELECT * FROM dbo.Categories;