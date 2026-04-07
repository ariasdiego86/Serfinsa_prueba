-- =============================================================
-- Script de inicialización de la base de datos: SerfinsaDb
-- SQL Server
-- Crea la base de datos, tablas (Users, Products), restricciones
-- y un usuario administrador inicial (seed).
-- =============================================================

-- Crear la base de datos si no existe
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'SerfinsaDb')
BEGIN
    CREATE DATABASE SerfinsaDb;
    PRINT 'Base de datos SerfinsaDb creada.';
END
GO

USE SerfinsaDb;
GO

-- =============================================================
-- Tabla: Users
-- Almacena los usuarios del sistema con sus credenciales hasheadas
-- y su rol para la autorización basada en roles.
-- =============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[Users] (
        [Id]           INT             NOT NULL IDENTITY(1,1),
        [Email]        NVARCHAR(256)   NOT NULL,
        [PasswordHash] NVARCHAR(MAX)   NOT NULL,
        -- Rol determina los permisos: 'Admin' tiene acceso total, 'User' solo lectura
        [Role]         NVARCHAR(50)    NOT NULL CONSTRAINT DF_Users_Role DEFAULT 'User',

        CONSTRAINT PK_Users PRIMARY KEY CLUSTERED ([Id] ASC)
    );

    -- Índice único en Email para evitar duplicados y agilizar búsquedas por email
    CREATE UNIQUE NONCLUSTERED INDEX UX_Users_Email
        ON [dbo].[Users] ([Email] ASC);

    PRINT 'Tabla Users creada.';
END
GO

-- =============================================================
-- Tabla: Products
-- Almacena el catálogo de productos con sus atributos de negocio.
-- =============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Products]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[Products] (
        [Id]           INT             NOT NULL IDENTITY(1,1),
        [Nombre]       NVARCHAR(200)   NOT NULL,
        [Descripcion]  NVARCHAR(1000)  NULL,
        -- Decimal(18,2) para precios: 16 dígitos enteros + 2 decimales, evita errores de redondeo
        [Precio]       DECIMAL(18,2)   NOT NULL,
        [Stock]        INT             NOT NULL CONSTRAINT DF_Products_Stock DEFAULT 0,
        [TipoProducto] NVARCHAR(100)   NULL,

        CONSTRAINT PK_Products PRIMARY KEY CLUSTERED ([Id] ASC),
        -- El stock no puede ser negativo
        CONSTRAINT CK_Products_Stock CHECK ([Stock] >= 0),
        -- El precio debe ser positivo
        CONSTRAINT CK_Products_Precio CHECK ([Precio] > 0)
    );

    PRINT 'Tabla Products creada.';
END
GO

-- =============================================================
-- Seed: Usuario Administrador por defecto
-- Contraseña: Admin123!
-- Hash generado con BCrypt (work factor 12).
-- IMPORTANTE: Cambia la contraseña después del primer login.
-- =============================================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[Users] WHERE [Email] = 'admin@serfinsa.com')
BEGIN
    INSERT INTO [dbo].[Users] ([Email], [PasswordHash], [Role])
    VALUES (
        'admin@serfinsa.com',
        -- Hash BCrypt de 'Admin123!' con work factor 12
        '$2a$12$LQv3c1yqBwEHxv8R1GlP8OCnORGp47iAkWUOegY.N7YYfEQwHEqGW',
        'Admin'
    );
    PRINT 'Usuario administrador creado: admin@serfinsa.com / Admin123!';
END
GO

-- =============================================================
-- Seed: Productos de ejemplo
-- =============================================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[Products])
BEGIN
    INSERT INTO [dbo].[Products] ([Nombre], [Descripcion], [Precio], [Stock], [TipoProducto])
    VALUES
        ('Laptop HP 15', 'Laptop HP 15 pulgadas, Intel Core i5, 8GB RAM, 256GB SSD', 799.99, 15, 'Electrónico'),
        ('Mouse Inalámbrico Logitech', 'Mouse ergonómico inalámbrico con batería recargable', 35.50, 50, 'Electrónico'),
        ('Teclado Mecánico Redragon', 'Teclado mecánico RGB con switches Blue', 65.00, 30, 'Electrónico'),
        ('Monitor Samsung 24"', 'Monitor Full HD 24 pulgadas, 75Hz, panel IPS', 220.00, 10, 'Electrónico'),
        ('Silla Gamer DXRacer', 'Silla ergonómica para gaming con soporte lumbar', 350.00, 5, 'Mobiliario');

    PRINT '5 productos de ejemplo insertados.';
END
GO

PRINT '=== Inicialización de SerfinsaDb completada exitosamente. ===';
GO
