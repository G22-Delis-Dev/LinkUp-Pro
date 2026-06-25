# 🔒 Guía de Implementación de Seguridad - LinkUp Pro

## 📋 Índice de Contenidos

1. [Autenticación y Autorización](#autenticación-y-autorización)
2. [Rutas Protegidas y Públicas](#rutas-protegidas-y-públicas)
3. [Validación de Estado de Cuenta](#validación-de-estado-de-cuenta)
4. [Autorización sobre Recursos](#autorización-sobre-recursos)
5. [Protección de Formularios](#protección-de-formularios)
6. [Seguridad de Contraseñas](#seguridad-de-contraseñas)
7. [Tokens de Activación y Restablecimiento](#tokens)
8. [Gestión de Sesiones](#gestión-de-sesiones)
9. [Protección de Contenido](#protección-de-contenido)
10. [Seguridad de Archivos](#seguridad-de-archivos)
11. [Control de Concurrencia](#control-de-concurrencia)
12. [Manejo de Errores](#manejo-de-errores)

---

## 1. Autenticación y Autorización

### ✅ Reglas Generales

- **Todas las rutas internas requieren `[Authorize]`**
- **Solo rutas públicas específicas usan `[AllowAnonymous]`**
- **La cuenta debe estar autenticada Y activa**
- **Validación en servidor, nunca solo en UI**

### 📝 Implementación Básica

```csharp
// ❌ INCORRECTO - AllowAnonymous en todo el controlador
[AllowAnonymous]
public class AccountController : Controller
{
    public IActionResult Login() { }
    public IActionResult Register() { }
    public IActionResult Profile() { } // ❌ Accesible sin autenticar
}

// ✅ CORRECTO - Authorize por defecto, AllowAnonymous específico
[Authorize]
public class AccountController : Controller
{
    [AllowAnonymous]
    public IActionResult Login() { }
    
    [AllowAnonymous]
    public IActionResult Register() { }
    
    // Profile requiere autenticación (hereda [Authorize])
    public IActionResult Profile() { }
}
```

---

## 2. Rutas Protegidas y Públicas

### 🔒 Rutas PROTEGIDAS (requieren autenticación)

```csharp
[Authorize]
public class HomeController : Controller { }

[Authorize]
public class PostController : Controller { }

[Authorize]
public class FriendshipController : Controller { }

[Authorize]
public class FriendRequestController : Controller { }

[Authorize]
public class NotificationController : Controller { }

[Authorize]
public class BattleshipController : Controller { }

[Authorize]
public class CommentController : Controller { }

[Authorize]
public class ReactionController : Controller { }

[Authorize]
public class ProfileController : Controller { }
```

### 🌐 Rutas PÚBLICAS (sin autenticación)

```csharp
public class AuthController : Controller
{
    [AllowAnonymous]
    public IActionResult Login() { }
    
    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model) { }
    
    [AllowAnonymous]
    public IActionResult Register() { }
    
    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model) { }
    
    [AllowAnonymous]
    public async Task<IActionResult> ActivateAccount(string userId, string token) { }
    
    [AllowAnonymous]
    public IActionResult ResendActivationLink() { }
    
    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResendActivationLink(ResendActivationViewModel model) { }
    
    [AllowAnonymous]
    public IActionResult ForgotPassword() { }
    
    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model) { }
    
    [AllowAnonymous]
    public IActionResult ResetPassword(string userId, string token) { }
    
    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model) { }
}
```

### 🔄 Redirección de Usuarios Autenticados

```csharp
[AllowAnonymous]
public IActionResult Login()
{
    // Si ya está autenticado, redirigir a Home
    if (User.Identity?.IsAuthenticated == true)
    {
        return RedirectToAction("Index", "Home");
    }
    
    return View();
}

[AllowAnonymous]
public IActionResult Register()
{
    // Si ya está autenticado, redirigir a Home
    if (User.Identity?.IsAuthenticated == true)
    {
        return RedirectToAction("Index", "Home");
    }
    
    return View();
}
```

---

## 3. Validación de Estado de Cuenta

### ⚠️ Estar autenticado NO es suficiente

La cuenta también debe estar **ACTIVA**.

### 📝 Implementación con Filtro Global

```csharp
// Filters/ActiveAccountFilter.cs
public class ActiveAccountFilter : IAsyncActionFilter
{
    private readonly UserManager<AppUser> _userManager;

    public ActiveAccountFilter(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context, 
        ActionExecutionDelegate next)
    {
        // Solo validar si está autenticado
        if (context.HttpContext.User.Identity?.IsAuthenticated == true)
        {
            var user = await _userManager.GetUserAsync(context.HttpContext.User);
            
            if (user == null || !user.IsActive)
            {
                // Cerrar sesión
                await context.HttpContext.SignOutAsync();
                
                // Redirigir al login con mensaje
                context.Result = new RedirectToActionResult(
                    "Login", 
                    "Auth", 
                    new { message = "Su cuenta no está activa." }
                );
                return;
            }
        }
        
        await next();
    }
}

// Program.cs
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<ActiveAccountFilter>();
});
```

---

## 4. Autorización sobre Recursos

### 📌 Principios Clave

1. ✅ Validar autenticación
2. ✅ Validar autorización sobre el recurso
3. ✅ Validar estado del recurso
4. ✅ Validar relaciones (amistad, participación, etc.)

### 🔒 Implementación por Módulo

#### **PUBLICACIONES**

```csharp
[Authorize]
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(Guid id, EditPostViewModel model)
{
    var userId = User.GetUserId(); // Extension method
    
    // 1. Obtener la publicación
    var post = await _postService.GetByIdAsync(id);
    if (post == null)
        return NotFound("Publicación no encontrada.");
    
    // 2. ✅ VALIDAR QUE EL USUARIO SEA EL AUTOR
    if (post.UserId != userId)
        return Forbid("No posee permisos para realizar esta acción.");
    
    // 3. Validar que esté activa
    if (post.IsDeleted)
        return BadRequest("No se puede editar una publicación eliminada.");
    
    // 4. Proceder con la edición
    await _postService.UpdateAsync(id, model);
    return RedirectToAction("Index");
}

[Authorize]
public async Task<IActionResult> Details(Guid id)
{
    var currentUserId = User.GetUserId();
    var post = await _postService.GetByIdAsync(id);
    
    if (post == null)
        return NotFound();
    
    // ✅ VALIDAR PRIVACIDAD
    if (post.Privacy == PostPrivacy.OnlyMe && post.UserId != currentUserId)
        return Forbid("No posee permisos para realizar esta acción.");
    
    if (post.Privacy == PostPrivacy.FriendsOnly)
    {
        // ✅ VALIDAR AMISTAD ACTIVA
        var areFriends = await _friendshipService
            .AreActiveFriendsAsync(currentUserId, post.UserId);
        
        if (!areFriends && post.UserId != currentUserId)
            return Forbid("No posee permisos para realizar esta acción.");
    }
    
    return View(post);
}
```

#### **COMENTARIOS**

```csharp
[Authorize]
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(CreateCommentViewModel model)
{
    var userId = User.GetUserId();
    
    // 1. Obtener la publicación
    var post = await _postService.GetByIdAsync(model.PostId);
    if (post == null)
        return NotFound();
    
    // 2. ✅ VALIDAR ACCESO A LA PUBLICACIÓN
    if (!await CanAccessPost(userId, post))
        return Forbid("No posee permisos para realizar esta acción.");
    
    // 3. ✅ VALIDAR QUE PERMITE COMENTARIOS
    if (!post.AllowComments)
        return BadRequest("La publicación no permite comentarios.");
    
    // 4. Crear comentario
    await _commentService.CreateAsync(userId, model);
    return RedirectToAction("Details", "Post", new { id = model.PostId });
}

[Authorize]
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Delete(Guid id)
{
    var userId = User.GetUserId();
    var comment = await _commentService.GetByIdAsync(id);
    
    if (comment == null)
        return NotFound();
    
    // ✅ VALIDAR QUE EL USUARIO SEA EL AUTOR
    if (comment.UserId != userId)
        return Forbid("No posee permisos para realizar esta acción.");
    
    await _commentService.DeleteAsync(id);
    return Ok();
}
```

#### **SOLICITUDES DE AMISTAD**

```csharp
[Authorize]
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Accept(Guid id)
{
    var userId = User.GetUserId();
    var request = await _friendRequestService.GetByIdAsync(id);
    
    if (request == null)
        return NotFound();
    
    // ✅ VALIDAR QUE EL USUARIO SEA EL RECEPTOR
    if (request.ReceiverId != userId)
        return Forbid("No posee permisos para realizar esta acción.");
    
    // Validar estado
    if (request.Status != FriendRequestStatus.Pending)
        return BadRequest("La solicitud ya fue procesada.");
    
    await _friendRequestService.AcceptAsync(id);
    return RedirectToAction("Index");
}

[Authorize]
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Cancel(Guid id)
{
    var userId = User.GetUserId();
    var request = await _friendRequestService.GetByIdAsync(id);
    
    if (request == null)
        return NotFound();
    
    // ✅ VALIDAR QUE EL USUARIO SEA EL EMISOR
    if (request.SenderId != userId)
        return Forbid("No posee permisos para realizar esta acción.");
    
    // Validar estado
    if (request.Status != FriendRequestStatus.Pending)
        return BadRequest("La solicitud ya fue procesada.");
    
    await _friendRequestService.CancelAsync(id);
    return RedirectToAction("Sent");
}
```

#### **NOTIFICACIONES**

```csharp
[Authorize]
public async Task<IActionResult> Index()
{
    var userId = User.GetUserId();
    
    // ✅ SOLO OBTENER NOTIFICACIONES DEL USUARIO ACTUAL
    var notifications = await _notificationService.GetByUserAsync(userId);
    
    return View(notifications);
}

[Authorize]
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> MarkAsRead(Guid id)
{
    var userId = User.GetUserId();
    var notification = await _notificationService.GetByIdAsync(id);
    
    if (notification == null)
        return NotFound();
    
    // ✅ VALIDAR QUE LA NOTIFICACIÓN PERTENEZCA AL USUARIO
    if (notification.UserId != userId)
        return Forbid("No posee permisos para realizar esta acción.");
    
    await _notificationService.MarkAsReadAsync(id);
    return Ok();
}
```

#### **BATTLESHIP**

```csharp
[Authorize]
public async Task<IActionResult> Board(Guid gameId)
{
    var userId = User.GetUserId();
    var game = await _battleshipService.GetGameAsync(gameId);
    
    if (game == null)
        return NotFound();
    
    // ✅ VALIDAR QUE EL USUARIO SEA PARTICIPANTE
    if (game.Player1Id != userId && game.Player2Id != userId)
        return Forbid("No posee permisos para realizar esta acción.");
    
    // ✅ NO MOSTRAR BARCOS DEL OPONENTE EN PARTIDA ACTIVA
    var board = await _battleshipService.GetMyBoardAsync(gameId, userId);
    
    return View(board);
}

[Authorize]
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Attack(AttackViewModel model)
{
    var userId = User.GetUserId();
    var game = await _battleshipService.GetGameAsync(model.GameId);
    
    if (game == null)
        return NotFound();
    
    // ✅ VALIDAR PARTICIPACIÓN
    if (game.Player1Id != userId && game.Player2Id != userId)
        return Forbid("No posee permisos para realizar esta acción.");
    
    // ✅ VALIDAR TURNO
    if (game.CurrentTurnPlayerId != userId)
        return BadRequest("No es su turno.");
    
    // ✅ VALIDAR ESTADO DEL JUEGO
    if (game.Status != GameStatus.InProgress)
        return BadRequest("La partida no está en curso.");
    
    // Procesar ataque
    var result = await _battleshipService.AttackAsync(model);
    return Ok(result);
}
```

#### **PERFIL**

```csharp
[Authorize]
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(EditProfileViewModel model)
{
    var userId = User.GetUserId();
    
    // ✅ VALIDAR QUE EDITE SU PROPIO PERFIL
    if (model.UserId != userId)
        return Forbid("No posee permisos para realizar esta acción.");
    
    await _userService.UpdateProfileAsync(userId, model);
    return RedirectToAction("Index");
}

[Authorize]
public async Task<IActionResult> ViewProfile(Guid userId)
{
    var currentUserId = User.GetUserId();
    
    // Puede ver su propio perfil
    if (userId == currentUserId)
    {
        var myProfile = await _userService.GetProfileAsync(userId);
        return View(myProfile);
    }
    
    // ✅ VALIDAR AMISTAD PARA VER PERFILES AJENOS
    var areFriends = await _friendshipService.AreActiveFriendsAsync(currentUserId, userId);
    if (!areFriends)
        return Forbid("No posee permisos para realizar esta acción.");
    
    var profile = await _userService.GetProfileAsync(userId);
    return View(profile);
}
```

---

## 5. Protección de Formularios

### 🛡️ Anti-Forgery Token

**TODAS las operaciones que modifican datos deben usar `[ValidateAntiForgeryToken]`**

```csharp
// ✅ CORRECTO
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(CreatePostViewModel model)
{
    // ...
}

// ❌ INCORRECTO - GET para modificar datos
[HttpGet]
public async Task<IActionResult> Delete(Guid id)
{
    await _service.DeleteAsync(id);
    return RedirectToAction("Index");
}

// ✅ CORRECTO - POST para modificar datos
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Delete(Guid id)
{
    await _service.DeleteAsync(id);
    return RedirectToAction("Index");
}
```

### 📝 En las Vistas

```html
<!-- Formularios -->
<form asp-action="Create" method="post">
    @Html.AntiForgeryToken()
    <!-- campos del formulario -->
    <button type="submit">Crear</button>
</form>

<!-- AJAX -->
<script>
$.ajax({
    url: '/Post/Delete',
    type: 'POST',
    data: {
        id: postId,
        __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
    },
    success: function(result) {
        // ...
    }
});
</script>
```

### 🚫 Protección contra Asignación Indebida

```csharp
// ❌ INCORRECTO - Binding directo de entidad
[HttpPost]
public IActionResult Edit(Post post) // ⚠️ Usuario puede modificar cualquier campo
{
    _context.Update(post);
    _context.SaveChanges();
    return RedirectToAction("Index");
}

// ✅ CORRECTO - Usar ViewModel
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(EditPostViewModel model)
{
    var userId = User.GetUserId();
    var post = await _postService.GetByIdAsync(model.Id);
    
    // Validar autorización
    if (post.UserId != userId)
        return Forbid();
    
    // ✅ Solo actualizar campos permitidos
    post.Content = model.Content;
    post.Privacy = model.Privacy;
    // UserId, CreatedAt, etc. NO se modifican
    
    await _postService.UpdateAsync(post);
    return RedirectToAction("Index");
}
```

---

## 6. Seguridad de Contraseñas

### 🔐 Configuración de Identity

```csharp
// Program.cs
builder.Services.AddIdentity<AppUser, IdentityRole<Guid>>(options =>
{
    // ✅ REQUISITOS DE CONTRASEÑA
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;
    
    // ✅ BLOQUEO POR INTENTOS FALLIDOS
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    
    // Validación de usuario
    options.User.RequireUniqueEmail = true;
    
    // SignIn
    options.SignIn.RequireConfirmedEmail = false; // Usar activación manual
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();
```

### 🔒 Login con Validación de Bloqueo

```csharp
[AllowAnonymous]
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Login(LoginViewModel model)
{
    if (!ModelState.IsValid)
        return View(model);
    
    var user = await _userManager.FindByNameAsync(model.Username);
    
    if (user == null)
    {
        ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
        return View(model);
    }
    
    // ✅ VALIDAR CUENTA ACTIVA
    if (!user.IsActive)
    {
        ModelState.AddModelError("", 
            "Su cuenta no está activa. Por favor, active su cuenta mediante el enlace enviado a su correo.");
        return View(model);
    }
    
    // ✅ VERIFICAR SI ESTÁ BLOQUEADA
    if (await _userManager.IsLockedOutAsync(user))
    {
        var lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);
        var remainingTime = lockoutEnd.Value - DateTimeOffset.UtcNow;
        
        ModelState.AddModelError("", 
            $"Su cuenta ha sido bloqueada temporalmente. Intente nuevamente en {remainingTime.Minutes} minutos.");
        return View(model);
    }
    
    // Intentar login
    var result = await _signInManager.PasswordSignInAsync(
        model.Username,
        model.Password,
        model.RememberMe,
        lockoutOnFailure: true // ✅ Activar bloqueo automático
    );
    
    if (result.Succeeded)
    {
        // ✅ RESTABLECER CONTADOR DE INTENTOS FALLIDOS
        await _userManager.ResetAccessFailedCountAsync(user);
        return RedirectToAction("Index", "Home");
    }
    
    if (result.IsLockedOut)
    {
        ModelState.AddModelError("", 
            "Su cuenta ha sido bloqueada temporalmente por múltiples intentos fallidos. Intente nuevamente en 15 minutos.");
        return View(model);
    }
    
    ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
    return View(model);
}
```

### 👁️ Campo de Contraseña con Toggle

```html
<div class="form-group">
    <label asp-for="Password">Contraseña</label>
    <div class="input-group">
        <input asp-for="Password" type="password" class="form-control" id="passwordField" />
        <div class="input-group-append">
            <button class="btn btn-outline-secondary" type="button" id="togglePassword">
                <i class="fas fa-eye" id="toggleIcon"></i>
            </button>
        </div>
    </div>
    <span asp-validation-for="Password" class="text-danger"></span>
</div>

<script>
document.getElementById('togglePassword').addEventListener('click', function() {
    const passwordField = document.getElementById('passwordField');
    const toggleIcon = document.getElementById('toggleIcon');
    
    if (passwordField.type === 'password') {
        passwordField.type = 'text';
        toggleIcon.classList.remove('fa-eye');
        toggleIcon.classList.add('fa-eye-slash');
    } else {
        passwordField.type = 'password';
        toggleIcon.classList.remove('fa-eye-slash');
        toggleIcon.classList.add('fa-eye');
    }
});
</script>
```

---

## 7. Tokens de Activación y Restablecimiento

### ⏰ Configuración de Tiempo de Vida

```csharp
// Program.cs
builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    // ✅ TOKENS DE ACTIVACIÓN: 24 horas
    options.TokenLifespan = TimeSpan.FromHours(24);
});
```

### 📧 Activación de Cuenta

```csharp
[AllowAnonymous]
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Register(RegisterViewModel model)
{
    if (!ModelState.IsValid)
        return View(model);
    
    var user = new AppUser
    {
        UserName = model.Username,
        Email = model.Email,
        FirstName = model.FirstName,
        LastName = model.LastName,
        IsActive = false, // ✅ INACTIVO HASTA ACTIVAR
        CreatedAt = DateTime.UtcNow
    };
    
    var result = await _userManager.CreateAsync(user, model.Password);
    
    if (!result.Succeeded)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }
        return View(model);
    }
    
    // ✅ GENERAR TOKEN DE ACTIVACIÓN (válido 24 horas)
    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
    
    // Enviar correo con enlace
    var activationLink = Url.Action(
        "ActivateAccount",
        "Auth",
        new { userId = user.Id, token = token },
        protocol: HttpContext.Request.Scheme
    );
    
    await _emailService.SendActivationEmailAsync(user.Email, activationLink);
    
    TempData["Message"] = "Registro exitoso. Por favor, revise su correo para activar su cuenta.";
    return RedirectToAction("Login");
}

[AllowAnonymous]
public async Task<IActionResult> ActivateAccount(string userId, string token)
{
    if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
        return BadRequest("Parámetros inválidos.");
    
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null)
        return NotFound("Usuario no encontrado.");
    
    // ✅ VALIDAR Y CONSUMIR TOKEN (uso único)
    var result = await _userManager.ConfirmEmailAsync(user, token);
    
    if (!result.Succeeded)
    {
        TempData["Error"] = "El enlace de activación es inválido o ha expirado.";
        return RedirectToAction("Login");
    }
    
    // ✅ ACTIVAR CUENTA
    user.IsActive = true;
    await _userManager.UpdateAsync(user);
    
    TempData["Success"] = "Su cuenta ha sido activada exitosamente. Ahora puede iniciar sesión.";
    return RedirectToAction("Login");
}
```

### 🔄 Reenvío de Enlace de Activación

```csharp
[AllowAnonymous]
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> ResendActivationLink(ResendActivationViewModel model)
{
    if (!ModelState.IsValid)
        return View(model);
    
    var user = await _userManager.FindByEmailAsync(model.Email);
    
    if (user == null || user.IsActive)
    {
        // ✅ NO REVELAR SI EL USUARIO EXISTE
        TempData["Message"] = "Si el correo es válido, recibirá un nuevo enlace de activación.";
        return RedirectToAction("Login");
    }
    
    // ✅ GENERAR NUEVO TOKEN
    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
    
    var activationLink = Url.Action(
        "ActivateAccount",
        "Auth",
        new { userId = user.Id, token = token },
        protocol: HttpContext.Request.Scheme
    );
    
    await _emailService.SendActivationEmailAsync(user.Email, activationLink);
    
    TempData["Message"] = "Si el correo es válido, recibirá un nuevo enlace de activación.";
    return RedirectToAction("Login");
}
```

### 🔑 Restablecimiento de Contraseña

```csharp
[AllowAnonymous]
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
{
    if (!ModelState.IsValid)
        return View(model);
    
    var user = await _userManager.FindByEmailAsync(model.Email);
    
    // ✅ NO REVELAR SI EL USUARIO EXISTE
    TempData["Message"] = "Si el correo es válido, recibirá un enlace para restablecer su contraseña.";
    
    if (user == null || !user.IsActive)
        return RedirectToAction("Login");
    
    // ✅ GENERAR TOKEN DE RESTABLECIMIENTO (válido 1 hora)
    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
    
    var resetLink = Url.Action(
        "ResetPassword",
        "Auth",
        new { userId = user.Id, token = token },
        protocol: HttpContext.Request.Scheme
    );
    
    await _emailService.SendPasswordResetEmailAsync(user.Email, resetLink);
    
    return RedirectToAction("Login");
}

[AllowAnonymous]
public async Task<IActionResult> ResetPassword(string userId, string token)
{
    if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
        return BadRequest("Parámetros inválidos.");
    
    var model = new ResetPasswordViewModel
    {
        UserId = userId,
        Token = token
    };
    
    return View(model);
}

[AllowAnonymous]
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
{
    if (!ModelState.IsValid)
        return View(model);
    
    var user = await _userManager.FindByIdAsync(model.UserId);
    if (user == null)
        return NotFound();
    
    // ✅ VALIDAR Y CONSUMIR TOKEN (uso único, válido 1 hora)
    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
    
    if (!result.Succeeded)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }
        return View(model);
    }
    
    // ✅ RESTABLECER CONTADOR DE INTENTOS FALLIDOS
    await _userManager.ResetAccessFailedCountAsync(user);
    
    TempData["Success"] = "Su contraseña ha sido restablecida exitosamente.";
    return RedirectToAction("Login");
}
```

### ⚙️ Configuración de Token Lifetime Personalizada

```csharp
// Para tokens de restablecimiento de contraseña con tiempo diferente
public class PasswordResetTokenProvider : DataProtectorTokenProvider<AppUser>
{
    public PasswordResetTokenProvider(
        IDataProtectionProvider dataProtectionProvider,
        IOptions<PasswordResetTokenProviderOptions> options,
        ILogger<DataProtectorTokenProvider<AppUser>> logger)
        : base(dataProtectionProvider, options, logger)
    {
    }
}

public class PasswordResetTokenProviderOptions : DataProtectionTokenProviderOptions
{
    public PasswordResetTokenProviderOptions()
    {
        Name = "PasswordResetTokenProvider";
        TokenLifespan = TimeSpan.FromHours(1); // ✅ 1 HORA
    }
}

// Program.cs
builder.Services.AddIdentity<AppUser, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddTokenProvider<PasswordResetTokenProvider>("PasswordReset");

builder.Services.Configure<PasswordResetTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromHours(1);
});
```


---

## 8. Gestión de Sesiones

### ⏰ Configuración de Sesiones

```csharp
// Program.cs
builder.Services.AddSession(options =>
{
    // ✅ TIEMPO DE INACTIVIDAD: 30 minutos
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    
    // ✅ COOKIES DE SESIÓN SEGURAS
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // HTTPS only
    options.Cookie.SameSite = SameSiteMode.Lax;
});

// Activar sesiones
var app = builder.Build();
app.UseSession();
```

### 🍪 Configuración de Cookies de Autenticación

```csharp
// Program.cs
builder.Services.ConfigureApplicationCookie(options =>
{
    // ✅ COOKIE HTTPONLY (no accesible desde JavaScript)
    options.Cookie.HttpOnly = true;
    
    // ✅ COOKIE SEGURA (solo HTTPS)
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    
    // ✅ SAMESITE (protección CSRF)
    options.Cookie.SameSite = SameSiteMode.Strict;
    
    // ✅ TIEMPO DE EXPIRACIÓN: 30 minutos de inactividad
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true; // Se renueva en cada petición
    
    // ✅ "REMEMBER ME": 7 días
    // (configurado en el login con isPersistent: true)
    
    // Rutas de autenticación
    options.LoginPath = "/Auth/Login";
    options.LogoutPath = "/Auth/Logout";
    options.AccessDeniedPath = "/Auth/AccessDenied";
});
```

### 🔐 Login con "Recordarme"

```csharp
[AllowAnonymous]
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Login(LoginViewModel model)
{
    // ... validaciones previas ...
    
    var result = await _signInManager.PasswordSignInAsync(
        model.Username,
        model.Password,
        isPersistent: model.RememberMe, // ✅ 7 días si marcó "Recordarme"
        lockoutOnFailure: true
    );
    
    if (result.Succeeded)
    {
        await _userManager.ResetAccessFailedCountAsync(user);
        return RedirectToAction("Index", "Home");
    }
    
    // ...
}
```

### 🚪 Logout Seguro

```csharp
[Authorize]
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Logout()
{
    await _signInManager.SignOutAsync();
    
    // Limpiar sesión
    HttpContext.Session.Clear();
    
    return RedirectToAction("Login", "Auth");
}
```

### 🔄 Renovación Automática de Sesión (Opcional)

```html
<!-- _Layout.cshtml -->
@if (User.Identity?.IsAuthenticated == true)
{
    <script>
        // Ping cada 5 minutos para mantener sesión activa
        setInterval(function() {
            fetch('/Auth/KeepAlive', {
                method: 'POST',
                headers: {
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value
                }
            });
        }, 5 * 60 * 1000); // 5 minutos
    </script>
}
```

```csharp
[Authorize]
[HttpPost]
public IActionResult KeepAlive()
{
    // Solo retornar OK para renovar el sliding expiration
    return Ok();
}
```

---

## 9. Protección de Contenido (XSS)

### 🛡️ Codificación Automática en Razor

```html
<!-- ✅ CORRECTO - Codificación automática -->
<p>@Model.UserInput</p>
<!-- Si contiene <script>, se mostrará como texto: &lt;script&gt; -->

<!-- ❌ INCORRECTO - HTML sin codificar -->
<p>@Html.Raw(Model.UserInput)</p>
<!-- ⚠️ ¡PELIGRO! Ejecuta scripts maliciosos -->
```

### 📝 Reglas de Codificación

```html
<!-- ✅ SIEMPRE usar @ para contenido de usuario -->
<div>
    <h3>@Model.Title</h3>
    <p>@Model.Content</p>
    <span>@Model.Username</span>
</div>

<!-- ✅ Atributos también se codifican automáticamente -->
<input type="text" value="@Model.Value" />
<a href="@Model.Link">Link</a>

<!-- ⚠️ NUNCA usar Html.Raw con contenido de usuario -->
@Html.Raw(Model.Content) <!-- ❌ PELIGROSO -->

<!-- ✅ Si DEBES permitir HTML (ej: editor rich text), SANITIZAR primero -->
@Html.Raw(HtmlSanitizer.Sanitize(Model.Content))
```

### 🧹 Sanitización de HTML (opcional para rich text)

```csharp
// Instalar: dotnet add package HtmlSanitizer

public class HtmlSanitizerService
{
    private readonly HtmlSanitizer _sanitizer;

    public HtmlSanitizerService()
    {
        _sanitizer = new HtmlSanitizer();
        
        // ✅ SOLO PERMITIR ETIQUETAS SEGURAS
        _sanitizer.AllowedTags.Clear();
        _sanitizer.AllowedTags.Add("p");
        _sanitizer.AllowedTags.Add("b");
        _sanitizer.AllowedTags.Add("i");
        _sanitizer.AllowedTags.Add("u");
        _sanitizer.AllowedTags.Add("strong");
        _sanitizer.AllowedTags.Add("em");
        _sanitizer.AllowedTags.Add("br");
        
        // ❌ NO PERMITIR script, iframe, object, embed, etc.
    }

    public string Sanitize(string html)
    {
        if (string.IsNullOrEmpty(html))
            return string.Empty;
        
        return _sanitizer.Sanitize(html);
    }
}

// Uso en controlador
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(CreatePostViewModel model)
{
    // ✅ SANITIZAR contenido HTML
    model.Content = _htmlSanitizer.Sanitize(model.Content);
    
    await _postService.CreateAsync(model);
    return RedirectToAction("Index");
}
```

### 🔒 Content Security Policy (CSP)

```csharp
// Program.cs
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Content-Security-Policy",
        "default-src 'self'; " +
        "script-src 'self'; " +
        "style-src 'self' 'unsafe-inline'; " +
        "img-src 'self' data: https:; " +
        "font-src 'self'; " +
        "connect-src 'self'; " +
        "frame-ancestors 'none';"
    );
    
    await next();
});
```

---

## 10. Seguridad de Archivos

### ✅ Validación Implementada

Ya implementado en `ImageValidator.cs`:

- ✅ Validación por magic numbers (contenido real)
- ✅ Solo .jpg, .jpeg, .png, .webp
- ✅ Tamaño máximo: 5 MB
- ✅ Nombres generados por GUID
- ✅ Almacenamiento fuera de wwwroot

### 📝 Uso en Controladores

```csharp
[Authorize]
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> UploadProfilePicture(IFormFile file)
{
    if (file == null || file.Length == 0)
    {
        ModelState.AddModelError("", "Debe seleccionar un archivo.");
        return View();
    }
    
    try
    {
        // ✅ VALIDAR IMAGEN (automático en SaveImageAsync)
        var userId = User.GetUserId();
        var imageUrl = await _imageStorageService.SaveImageAsync(file, userId.ToString());
        
        // Actualizar perfil
        await _userService.UpdateProfilePictureAsync(userId, imageUrl);
        
        return RedirectToAction("Profile");
    }
    catch (InvalidImageException ex)
    {
        ModelState.AddModelError("", ex.Message);
        return View();
    }
}
```

### 🔒 Servir Imágenes de Forma Segura

```csharp
[Authorize]
public async Task<IActionResult> GetImage(string filename)
{
    var userId = User.GetUserId();
    
    // ✅ VALIDAR AUTORIZACIÓN SOBRE LA IMAGEN
    var image = await _imageStorageService.GetImageAsync(filename);
    
    if (image == null)
        return NotFound();
    
    // Validar que el usuario tenga permiso de ver esta imagen
    // (ej: es su imagen o es amigo del dueño)
    
    return File(image.Stream, image.ContentType);
}
```

---

## 11. Control de Concurrencia

### ⚙️ Configuración en Entidades

```csharp
// Domain Entity
public class Post
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    
    // ✅ ROWVERSION para concurrencia
    [Timestamp]
    public byte[] RowVersion { get; set; }
}

// EF Configuration
public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.Property(p => p.RowVersion)
            .IsRowVersion();
    }
}
```

### 🔄 Manejo de Conflictos

```csharp
[Authorize]
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(EditPostViewModel model)
{
    var userId = User.GetUserId();
    
    try
    {
        var post = await _context.Posts.FindAsync(model.Id);
        
        if (post == null)
            return NotFound();
        
        // Validar autorización
        if (post.UserId != userId)
            return Forbid();
        
        // ✅ ACTUALIZAR CON ROWVERSION
        post.Content = model.Content;
        post.Privacy = model.Privacy;
        
        // EF compara RowVersion automáticamente
        await _context.SaveChangesAsync();
        
        return RedirectToAction("Details", new { id = model.Id });
    }
    catch (DbUpdateConcurrencyException)
    {
        // ✅ CONFLICTO DE CONCURRENCIA
        ModelState.AddModelError("",
            "La publicación fue modificada por otro usuario. " +
            "Por favor, recargue la página e intente nuevamente.");
        
        return View(model);
    }
}
```

### 🔁 Reintento Automático (Opcional)

```csharp
public async Task<bool> UpdatePostWithRetryAsync(Guid postId, string newContent, int maxRetries = 3)
{
    for (int attempt = 0; attempt < maxRetries; attempt++)
    {
        try
        {
            var post = await _context.Posts.FindAsync(postId);
            post.Content = newContent;
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException) when (attempt < maxRetries - 1)
        {
            // Recargar desde la base de datos
            await _context.Entry(post).ReloadAsync();
            // Reintentar
        }
    }
    
    return false;
}
```

---

## 12. Manejo de Errores

### 🚨 Middleware de Manejo de Errores

```csharp
// Middleware/ErrorHandlingMiddleware.cs
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    public ErrorHandlingMiddleware(
        RequestDelegate next,
        ILogger<ErrorHandlingMiddleware> logger,
        IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ocurrió un error no controlado.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "text/html";

        // ✅ NO EXPONER DETALLES TÉCNICOS EN PRODUCCIÓN
        if (_env.IsDevelopment())
        {
            return context.Response.WriteAsync(
                $"<h1>Error</h1><p>{exception.Message}</p><pre>{exception.StackTrace}</pre>");
        }
        else
        {
            // ✅ MENSAJE GENÉRICO EN PRODUCCIÓN
            return context.Response.WriteAsync(
                "<h1>Error</h1><p>Ha ocurrido un error. Por favor, intente nuevamente más tarde.</p>");
        }
    }
}

// Program.cs
app.UseMiddleware<ErrorHandlingMiddleware>();
```

### 📄 Páginas de Error Personalizadas

```csharp
// Program.cs
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/Index");
    app.UseStatusCodePagesWithReExecute("/Error/{0}");
    app.UseHsts();
}

// ErrorController.cs
public class ErrorController : Controller
{
    [AllowAnonymous]
    public IActionResult Index(int? statusCode = null)
    {
        if (statusCode.HasValue)
        {
            if (statusCode == 404)
                return View("NotFound");
            
            if (statusCode == 403)
                return View("Forbidden");
            
            if (statusCode == 500)
                return View("ServerError");
        }
        
        return View("Error");
    }
    
    [AllowAnonymous]
    [Route("Error/{statusCode}")]
    public IActionResult HandleError(int statusCode)
    {
        return Index(statusCode);
    }
}
```

### 🔒 Logging sin Datos Sensibles

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Login(LoginViewModel model)
{
    try
    {
        // ✅ NO LOGGEAR CONTRASEÑAS
        _logger.LogInformation("Intento de login para usuario: {Username}", model.Username);
        
        var result = await _signInManager.PasswordSignInAsync(
            model.Username,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: true
        );
        
        if (result.Succeeded)
        {
            _logger.LogInformation("Login exitoso para usuario: {Username}", model.Username);
            return RedirectToAction("Index", "Home");
        }
        
        // ✅ NO REVELAR SI EL USUARIO EXISTE
        _logger.LogWarning("Login fallido para usuario: {Username}", model.Username);
        ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
        
        return View(model);
    }
    catch (Exception ex)
    {
        // ✅ LOGGEAR EXCEPCIONES SIN EXPONER AL USUARIO
        _logger.LogError(ex, "Error en login para usuario: {Username}", model.Username);
        ModelState.AddModelError("", "Ha ocurrido un error. Por favor, intente nuevamente.");
        return View(model);
    }
}
```

### ⚠️ Validación de ModelState

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(CreatePostViewModel model)
{
    if (!ModelState.IsValid)
    {
        // ✅ RETORNAR ERRORES DE VALIDACIÓN
        return View(model);
    }
    
    try
    {
        await _postService.CreateAsync(model);
        return RedirectToAction("Index");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al crear publicación");
        
        // ✅ MENSAJE GENÉRICO AL USUARIO
        ModelState.AddModelError("", "No se pudo crear la publicación. Intente nuevamente.");
        return View(model);
    }
}
```

---

## 📚 Resumen de Implementación

### ✅ Checklist de Seguridad

#### Autenticación y Autorización
- [ ] `[Authorize]` en todos los controladores internos
- [ ] `[AllowAnonymous]` solo en rutas públicas específicas
- [ ] Validación de cuenta activa (ActiveAccountFilter)
- [ ] Validación de autorización sobre recursos

#### Formularios y CSRF
- [ ] `[ValidateAntiForgeryToken]` en todos los POST
- [ ] `@Html.AntiForgeryToken()` en todos los formularios
- [ ] ViewModels para prevenir over-posting

#### Contraseñas y Tokens
- [ ] Requisitos de contraseña: 8+ chars, upper, lower, digit, special
- [ ] Bloqueo por intentos fallidos: 5 intentos, 15 min
- [ ] Tokens de activación: 24 horas
- [ ] Tokens de reset: 1 hora
- [ ] Tokens de uso único

#### Sesiones
- [ ] Tiempo de inactividad: 30 minutos
- [ ] "Recordarme": 7 días
- [ ] Cookies HttpOnly, Secure, SameSite

#### Contenido
- [ ] Codificación automática con `@Model.Property`
- [ ] NUNCA `Html.Raw` con contenido de usuario
- [ ] Sanitización de HTML si se permite rich text
- [ ] Content Security Policy (CSP)

#### Archivos
- [ ] Validación por magic numbers
- [ ] Solo imágenes permitidas
- [ ] Tamaño máximo: 5 MB
- [ ] Nombres por GUID

#### Errores
- [ ] Middleware de manejo de errores
- [ ] Mensajes genéricos en producción
- [ ] Logging sin datos sensibles
- [ ] Páginas de error personalizadas

---

## 🔗 Archivos Relacionados

- `ImageValidator.cs` - Validación de archivos ya implementada
- `LocalImageStorageService.cs` - Servicio de almacenamiento seguro
- `InvalidImageException.cs` - Excepción para validación

---

**Fecha de creación:** 2026-06-24  
**Versión:** 1.0  
**Autor:** Kiro AI Assistant
