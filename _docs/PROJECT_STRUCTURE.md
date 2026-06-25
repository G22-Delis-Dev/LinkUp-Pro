# 📁 Estructura Completa del Proyecto LinkUp Pro

## 🎯 Estructura Visual

```
LinkUp Pro/
│
├── 📂 src/                                    # Código fuente
│   │
│   ├── 📂 Core/                               # ⭐ NÚCLEO - Sin dependencias externas
│   │   │
│   │   ├── 📦 LinkUpPro.Core.Domain/         # Entidades de dominio puras
│   │   │   ├── Entities/                     # User, Post, Comment, Friendship, etc.
│   │   │   ├── Enums/                        # Estados, tipos, privacidad, etc.
│   │   │   └── Interfaces/Repositories/      # Contratos de repositorios
│   │   │
│   │   └── 📦 LinkUpPro.Core.Application/    # Lógica de aplicación
│   │       ├── DTOs/                         # Objetos de transferencia de datos
│   │       ├── Interfaces/Services/          # Contratos de servicios
│   │       └── Common/                       # Utilidades compartidas
│   │
│   ├── 📂 Infrastructure/                     # 🔧 INFRAESTRUCTURA - Implementaciones
│   │   │
│   │   ├── 📦 LinkUpPro.Infrastructure.Identity/      # ASP.NET Core Identity
│   │   │   ├── Entities/                              # AppUser (IdentityUser)
│   │   │   ├── Configurations/                        # Configuración de Identity
│   │   │   └── Services/                              # Servicios de autenticación
│   │   │
│   │   ├── 📦 LinkUpPro.Infrastructure.Persistence/   # Acceso a datos
│   │   │   ├── Context/                               # ApplicationDbContext
│   │   │   ├── Configurations/                        # Configuraciones EF Core
│   │   │   ├── Repositories/                          # Implementación de repositorios
│   │   │   └── Migrations/                            # Migraciones de BD
│   │   │
│   │   └── 📦 LinkUpPro.Infrastructure.Shared/        # Servicios compartidos
│   │       └── Services/Storage/                      # Almacenamiento de imágenes
│   │           ├── IImageStorageService.cs
│   │           ├── LocalImageStorageService.cs
│   │           ├── ImageValidator.cs
│   │           └── InvalidImageException.cs
│   │
│   └── 📂 Presentation/                       # 🌐 CAPA DE PRESENTACIÓN
│       │
│       └── 📦 LinkUpPro.Web/                  # Aplicación web ASP.NET Core MVC
│           ├── Controllers/                   # Controladores MVC
│           ├── Views/                         # Vistas Razor
│           ├── ViewModels/                    # Modelos de vista
│           ├── Filters/                       # Filtros personalizados
│           │   └── ActiveAccountFilter.cs
│           ├── Middleware/                    # Middleware personalizado
│           │   └── ErrorHandlingMiddleware.cs
│           ├── Extensions/                    # Métodos de extensión
│           │   └── UserExtensions.cs
│           ├── wwwroot/                       # Archivos estáticos (CSS, JS, imágenes)
│           ├── appsettings.json              # Configuración
│           └── Program.cs                     # Punto de entrada
│
├── 📂 _docs/                                  # 📚 DOCUMENTACIÓN
│   ├── README.md                              # Índice de documentación
│   ├── ARCHITECTURE_REVIEW.md                 # Revisión de arquitectura
│   ├── ARCHITECTURE_SUMMARY.md                # Resumen de arquitectura
│   ├── SECURITY_IMPLEMENTATION_GUIDE.md       # Guía de seguridad completa
│   ├── SECURITY_IMPLEMENTATION_ROADMAP.md     # Plan de implementación de seguridad
│   ├── MIGRATION_NET9.md                      # Documentación de migración a .NET 9
│   ├── NET9_QUICK_SUMMARY.txt                 # Resumen rápido de migración
│   ├── PROJECT_STRUCTURE.md                   # Este archivo
│   └── build_*.txt                            # Logs de compilación
│
├── 📄 LinkUp Pro.sln                          # Solución de Visual Studio
├── 📄 README.md                               # README principal del proyecto
├── 📄 .gitignore                              # Archivos ignorados por Git
└── 📄 .gitattributes                          # Configuración de Git

```

## 🔄 Flujo de Dependencias (Arquitectura Onion)

```
┌─────────────────────────────────────────────────────────────┐
│                                                             │
│                    🌐 Presentation Layer                     │
│                     (LinkUpPro.Web)                         │
│                                                             │
│  Controllers → ViewModels → Views                          │
│       ↓                                                     │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                                                             │
│                 🔧 Infrastructure Layer                      │
│                                                             │
│  ┌───────────────┐  ┌──────────────┐  ┌─────────────────┐ │
│  │   Identity    │  │ Persistence  │  │     Shared      │ │
│  │ (Auth/Users)  │  │ (EF Core/DB) │  │  (Services)     │ │
│  └───────────────┘  └──────────────┘  └─────────────────┘ │
│           ↓                 ↓                  ↓            │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                                                             │
│                    ⭐ Core Layer (Núcleo)                    │
│                                                             │
│  ┌────────────────────────┐  ┌──────────────────────────┐ │
│  │   Application          │  │      Domain              │ │
│  │   (Services, DTOs)     │  │  (Entities, Interfaces)  │ │
│  └────────────────────────┘  └──────────────────────────┘ │
│              ↓                           ↑                 │
│              └───────────────────────────┘                 │
│                    (Sin dependencias)                      │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

## 📊 Dependencias entre Proyectos

### Presentation (Web)
- ✅ Referencia a: **Application**, **Domain**, **Identity**, **Persistence**, **Shared**
- ❌ No depende de: Nada externo

### Infrastructure (Identity, Persistence, Shared)
- ✅ Identity referencia a: **Domain**
- ✅ Persistence referencia a: **Domain**, **Application**, **Identity** (*)
- ✅ Shared referencia a: **Domain**
- (*) Excepción documentada por ApplicationDbContext

### Core (Application, Domain)
- ✅ Application referencia a: **Domain**
- ✅ Domain: **Sin dependencias** (núcleo puro)

## 🎯 Responsabilidades por Capa

### 📦 Core.Domain
- Entidades de dominio (User, Post, Comment, etc.)
- Enumeraciones (Status, Privacy, etc.)
- Interfaces de repositorios
- Lógica de negocio en entidades
- **Regla**: Sin dependencias externas

### 📦 Core.Application
- DTOs (Data Transfer Objects)
- Interfaces de servicios
- Lógica de aplicación
- Validaciones de negocio
- **Regla**: Solo depende de Domain

### 📦 Infrastructure.Identity
- Configuración de ASP.NET Core Identity
- AppUser (extiende IdentityUser)
- Servicios de autenticación
- Gestión de usuarios y roles

### 📦 Infrastructure.Persistence
- ApplicationDbContext (EF Core)
- Implementación de repositorios
- Configuraciones de entidades (Fluent API)
- Migraciones de base de datos

### 📦 Infrastructure.Shared
- Servicios de almacenamiento de imágenes
- Servicios de correo electrónico
- Utilidades compartidas
- Validadores de archivos

### 📦 Presentation.Web
- Controladores MVC
- Vistas Razor
- ViewModels
- Filtros y Middleware personalizados
- Configuración de la aplicación (Program.cs)
- Punto de entrada de la aplicación

## 🔒 Características de Seguridad Implementadas

### ✅ En Program.cs
- Configuración de Identity con requisitos de contraseña
- Configuración de bloqueo de cuenta (5 intentos, 15 min)
- Cookies HttpOnly, Secure, SameSite
- Sesiones seguras (30 min inactividad)
- Content Security Policy (CSP)
- Anti-forgery tokens
- Middleware de manejo de errores

### ✅ Filtros y Middleware
- **ActiveAccountFilter**: Valida que las cuentas estén activas
- **ErrorHandlingMiddleware**: Manejo centralizado de errores
- **UserExtensions**: Helpers para obtener datos del usuario

### ✅ Validación de Imágenes
- **ImageValidator**: Validación por magic numbers
- Formatos permitidos: JPG, PNG, WebP
- Tamaño máximo: 5 MB
- Nombres generados por GUID

## 📝 Próximos Pasos

1. ✅ **Estructura del proyecto** - Completado
2. ✅ **Configuración de seguridad** - Completado
3. 🚧 **Implementar ViewModels** - Pendiente
4. 🚧 **Implementar Controllers** - Pendiente
5. 🚧 **Implementar Views** - Pendiente
6. 🚧 **Servicios de negocio** - Pendiente

---

**Fecha**: 2026-06-24  
**Versión**: .NET 9  
**Arquitectura**: Onion (Clean Architecture)
