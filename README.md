# Serfinsa — Prueba Técnica Full Stack

Aplicación full-stack con **ASP.NET Core .NET 10** (Clean Architecture) y **React 19** (Vite + pnpm).

---

## Arquitectura

```
Serfinsa_prueba_tecnica/
├── backend/                     # Solución .NET 10 (Clean Architecture)
│   ├── Serfinsa.slnx            # Archivo de solución (formato nuevo .NET 9+)
│   ├── Serfinsa.Domain/         # Entidades, excepciones, interfaces de repos
│   ├── Serfinsa.Application/    # Casos de uso, DTOs, AutoMapper, FluentValidation
│   ├── Serfinsa.Infrastructure/ # EF Core, repositorios, JWT, BCrypt
│   ├── Serfinsa.Api/            # Controllers, Middleware, Swagger, Program.cs
│   └── Serfinsa.Tests/          # Pruebas unitarias xUnit + Moq
├── frontend/                    # React 19 + Vite + pnpm
│   ├── pnpm-workspace.yaml      # Seguridad supply chain (minimumReleaseAge: 2880)
│   └── src/
│       ├── contexts/AuthContext.jsx  # Estado global de autenticación
│       ├── components/              # ProtectedRoute, Navbar
│       ├── pages/                   # Login, Products (CRUD)
│       └── services/api.js          # Axios + interceptores JWT
├── database_init.sql            # Script SQL Server (tablas + seed)
└── README.md
```

---

## Requisitos previos

| Herramienta | Versión mínima |
|-------------|---------------|
| .NET SDK | 10.0 |
| SQL Server | 2019+ (o SQL Server Express) |
| Node.js | 20+ |
| pnpm | 9+ |

Instalar pnpm si no lo tienes:
```bash
npm install -g pnpm
```

---

## 1. Configurar la Base de Datos

### Opción A: Script SQL (recomendado para la prueba)

1. Abre SQL Server Management Studio (SSMS) o Azure Data Studio.
2. Ejecuta el archivo `database_init.sql`:
   ```sql
   -- En SSMS: File > Open > database_init.sql, luego F5
   ```
   Esto crea la base de datos `SerfinsaDb`, las tablas y el usuario administrador por defecto.

### Opción B: EF Core Migrations

```bash
cd backend

# Instalar herramienta EF Core si no la tienes
dotnet tool install --global dotnet-ef

# Crear la base de datos con migraciones
dotnet ef database update --project Serfinsa.Infrastructure --startup-project Serfinsa.Api
```

### Usuario por defecto (seed)
| Campo | Valor |
|-------|-------|
| Email | `admin@serfinsa.com` |
| Contraseña | `Admin123!` |
| Rol | `Admin` |

> **Importante**: Cambia la contraseña en producción.

---

## 2. Configurar el Backend

### Cadena de conexión

Edita `backend/Serfinsa.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=TU_SERVIDOR;Database=SerfinsaDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "JwtSettings": {
    "SecretKey": "TuClaveSecretaMuySeguraDeAlMenos32Caracteres!2024",
    "Issuer": "SerfinsaApi",
    "Audience": "SerfinsaClient",
    "ExpirationMinutes": "60"
  }
}
```

- Para SQL Server local con Windows Auth: `Server=localhost;Database=SerfinsaDb;Trusted_Connection=True;TrustServerCertificate=True;`
- Para SQL Server con usuario/contraseña: `Server=localhost;Database=SerfinsaDb;User Id=sa;Password=TuPassword;TrustServerCertificate=True;`

### Ejecutar el Backend

```bash
cd backend/Serfinsa.Api
dotnet run
```

La API estará disponible en:
- HTTP: `http://localhost:5000`
- Swagger UI: `http://localhost:5000/swagger`

---

## 3. Ejecutar el Frontend

```bash
cd frontend
pnpm install
pnpm dev
```

La aplicación estará en: `http://localhost:5173`

---

## 4. Ejecutar las Pruebas Unitarias

```bash
cd backend
dotnet test Serfinsa.Tests
```

Las pruebas cubren:
- `AuthServiceTests`: Login exitoso, usuario no encontrado, contraseña incorrecta.
- `ProductServiceTests`: GetAll, GetById, Create, Delete y Update con casos de error.

---

## 5. Probar la API con Swagger

1. Abre `http://localhost:5000/swagger`
2. Usa el endpoint `POST /api/auth/login` con:
   ```json
   {
     "email": "admin@serfinsa.com",
     "password": "Admin123!"
   }
   ```
3. Copia el `token` de la respuesta.
4. Haz clic en **Authorize** (botón en la esquina superior derecha de Swagger).
5. Ingresa: `Bearer TU_TOKEN_AQUI`
6. Ahora puedes probar todos los endpoints de `/api/products`.

---

## Decisiones técnicas

| Aspecto | Decisión | Motivo |
|---------|----------|--------|
| **Arquitectura** | Clean Architecture (4 capas) | Desacoplamiento, testabilidad, SOLID |
| **ORM** | EF Core con SQL Server | Tipado fuerte, migraciones, LINQ |
| **Hash de contraseñas** | BCrypt (work factor 12) | Resistente a fuerza bruta, salt automático |
| **JWT** | HS256 con claims de rol | Estándar, sin estado en servidor |
| **Validación backend** | FluentValidation | Declarativo, reutilizable, testeable |
| **Mapeo** | AutoMapper | Separa entidades de DTOs, reduce boilerplate |
| **Errores** | GlobalExceptionHandler + ProblemDetails (RFC 7807) | Contrato consistente, manejable en frontend |
| **Gestión de estado** | React Context + localStorage | Sin dependencias extra, sencillo para este scope |
| **HTTP cliente** | Axios con interceptores | Inyección automática de JWT, manejo global de 401 |
| **Seguridad supply chain** | `minimumReleaseAge: 2880` en pnpm-workspace.yaml | Mitiga paquetes maliciosos recién publicados |
