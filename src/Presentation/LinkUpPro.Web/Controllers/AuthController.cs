using LinkUpPro.Application.DTOs.Auth;
using LinkUpPro.Application.Interfaces.Identity;
using LinkUpPro.Application.ViewModels.Auth;
using Microsoft.AspNetCore.Mvc;

namespace LinkUpPro.Web.Controllers;

public class AuthController : Controller
{
    private readonly ILoginService _loginService;
    private readonly IRegisterService _registerService;
    private readonly IAccountActivationService _activationService;
    private readonly IPasswordResetService _passwordResetService;
    private readonly ISessionService _sessionService;

    public AuthController(
        ILoginService loginService,
        IRegisterService registerService,
        IAccountActivationService activationService,
        IPasswordResetService passwordResetService,
        ISessionService sessionService)
    {
        _loginService = loginService;
        _registerService = registerService;
        _activationService = activationService;
        _passwordResetService = passwordResetService;
        _sessionService = sessionService;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null, string? message = null)
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
            return RedirectToAction("Index", "Home");

        if (!string.IsNullOrEmpty(message))
        {
            TempData["Error"] = message;
        }
        else if (!string.IsNullOrEmpty(returnUrl))
        {
            TempData["Info"] = "Debe iniciar sesión para acceder a esta sección.";
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
            return View(model);

        var dto = new LoginDto
        {
            Username = model.Username,
            Password = model.Password
        };

        var result = await _loginService.LoginAsync(dto);
        
        if (!result.HasError)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
                
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, result.Error ?? "Error al iniciar sesión.");
        
        if (result.Error?.Contains("activada") == true)
        {
            ViewBag.ShowResendActivation = true;
            ViewBag.Username = model.Username;
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
            return RedirectToAction("Index", "Home");

        return View(new RegisterViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var dto = new RegisterDto
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Username = model.Username,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            Password = model.Password,
            ConfirmPassword = model.ConfirmPassword,
            Origin = $"{Request.Scheme}://{Request.Host.Value}"
        };

        var result = await _registerService.RegisterAsync(dto);

        if (!result.HasError)
        {
            return RedirectToAction(nameof(AccountCreated), new { email = model.Email });
        }

        ModelState.AddModelError(string.Empty, result.Error ?? "Error en el registro.");
        return View(model);
    }

    public IActionResult AccountCreated(string email)
    {
        ViewBag.Email = email;
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Activate(string userId, string token)
    {
        var result = await _activationService.ActivateAsync(userId, token);

        if (!result.HasError)
        {
            TempData["Success"] = "Cuenta activada exitosamente. Ya puedes iniciar sesión.";
            return RedirectToAction(nameof(Login));
        }

        TempData["Error"] = result.Error ?? "Error al activar la cuenta.";
        return RedirectToAction(nameof(Login));
    }

    [HttpPost]
    public async Task<IActionResult> ResendActivation(string username)
    {
        // En una app real, si sólo tenemos username del form, buscaríamos el email. 
        // Si el servicio acepta email, lo pasamos (aquí lo mapeamos simple a username porque a veces Auth los unifica).
        var origin = $"{Request.Scheme}://{Request.Host.Value}";
        var result = await _activationService.ResendActivationAsync(username, origin);

        if (!result.HasError)
            TempData["Success"] = "Se ha reenviado el enlace de activación a su correo electrónico.";
        else
            TempData["Error"] = result.Error;

        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View(new ForgotPasswordViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var dto = new ForgotPasswordDto { Email = model.Email, Origin = $"{Request.Scheme}://{Request.Host.Value}" };
        var result = await _passwordResetService.RequestResetAsync(dto);

        TempData["Info"] = "Si el correo existe en nuestro sistema, recibirá un enlace para restablecer su contraseña.";
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult ResetPassword(string token, string email)
    {
        var model = new ResetPasswordViewModel { Token = token, Email = email };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var dto = new ResetPasswordDto
        {
            Email = model.Email,
            Token = model.Token,
            NewPassword = model.NewPassword,
            ConfirmPassword = model.ConfirmPassword
        };

        var result = await _passwordResetService.ResetPasswordAsync(dto);

        if (!result.HasError)
        {
            TempData["Success"] = "Contraseña restablecida exitosamente. Ya puedes iniciar sesión.";
            return RedirectToAction(nameof(Login));
        }

        ModelState.AddModelError(string.Empty, result.Error ?? "Error al restablecer la contraseña.");
        return View(model);
    }

    public async Task<IActionResult> Logout()
    {
        await _sessionService.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}
