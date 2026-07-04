# LinkUp Pro

Red social desarrollada con arquitectura Onion (Clean Architecture) en .NET 9.

## 📁 Estructura del Proyecto

```
LinkUp Pro/
├── src/
│   ├── Core/                          # Capa de dominio (núcleo)
│   │   ├── LinkUpPro.Core.Domain/     # Entidades, interfaces de repositorios
│   │   └── LinkUpPro.Core.Application/ # DTOs, interfaces de servicios, lógica de negocio
│   │
│   ├── Infrastructure/                # Capa de infraestructura
│   │   ├── LinkUpPro.Infrastructure.Identity/     # Autenticación con Identity
│   │   ├── LinkUpPro.Infrastructure.Persistence/  # Implementación de repositorios, EF Core
│   │   └── LinkUpPro.Infrastructure.Shared/       # Servicios compartidos (almacenamiento, etc.)
│   │
│   └── Presentation/                  # Capa de presentación
│       └── LinkUpPro.Web/             # Aplicación web ASP.NET Core MVC
│
├── _docs/                             # Documentación del proyecto
└── LinkUp Pro.sln                     # Solución de Visual Studio
```

## 🏗️ Arquitectura

El proyecto implementa **arquitectura Onion** (Clean Architecture) con las siguientes capas:

### Core (Núcleo)
- **Domain**: Entidades de dominio, enumeraciones, interfaces de repositorios
- **Application**: DTOs, interfaces de servicios, lógica de aplicación

### Infrastructure (Infraestructura)
- **Identity**: Gestión de autenticación y usuarios con ASP.NET Core Identity
- **Persistence**: Implementación de repositorios, DbContext, configuraciones de EF Core
- **Shared**: Servicios compartidos (almacenamiento de imágenes, emails, etc.)

### Presentation (Presentación)
- **Web**: Aplicación web MVC, controladores, vistas, ViewModels

## ⚙️ Tecnologías

- **.NET 9**
- **ASP.NET Core MVC**
- **Entity Framework Core 9**
- **ASP.NET Core Identity**
- **SQL Server**

## 🔒 Características de Seguridad

- Autenticación con Identity
- Validación de cuentas activas
- Contraseñas seguras (8+ caracteres, mayúsculas, minúsculas, dígitos, caracteres especiales)
- Bloqueo de cuenta (5 intentos fallidos, 15 minutos)
- Tokens de activación (24 horas) y restablecimiento (1 hora)
- Sesiones seguras (30 minutos de inactividad)
- Cookies HttpOnly, Secure, SameSite
- Anti-forgery tokens en formularios
- Content Security Policy (CSP)
- Validación de imágenes por magic numbers
- Control de concurrencia optimista

## 📚 Documentación

Ver la carpeta `_docs/` para documentación detallada sobre:
- Arquitectura y diseños
- Guías de seguridad
- Migraciones y actualizaciones

## 🚀 Inicio Rápido

1. Clonar el repositorio
2. Configurar la cadena de conexión en `src/Presentation/LinkUpPro.Web/appsettings.json`
3. Ejecutar migraciones: `dotnet ef database update`
4. Ejecutar la aplicación: `dotnet run --project src/Presentation/LinkUpPro.Web`

---


**Versión**: .NET 9  
**Última actualización**: 2026-06-24
