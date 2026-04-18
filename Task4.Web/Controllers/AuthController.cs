using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Task4.Web.Models;
using Task4.Web.Services;
using Task4.Web.ViewModels;

namespace Task4.Web.Controllers
{
    public sealed class AuthController(AuthService authService) : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Users");

            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await authService.FindForLoginAsync(CreateLoginCommand(model), cancellationToken);

            if (user is null)
                return InvalidLogin(model);

            await SignInAsync(user);
            await authService.RecordLoginAsync(user, cancellationToken);

            return RedirectToAction("Index", "Users");
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Users");

            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return View(model);

            var request = HttpContext.Request;

            var result = await authService.RegisterAsync(CreateRegisterCommand(model), request.Scheme, request.Host.Value!, cancellationToken);

            if (!result.Succeeded)
                return RegistrationFailed(model, result.Error);

            TempData["StatusMessage"] = "Registration completed. Please confirm your e-mail.";
            return RedirectToAction(nameof(Login));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        private static LoginCommand CreateLoginCommand(LoginViewModel model)
        {
            return new LoginCommand
            {
                Email = model.Email,
                Password = model.Password
            };
        }

        private static RegisterUserCommand CreateRegisterCommand(RegisterViewModel model)
        {
            return new RegisterUserCommand
            {
                Name = model.Name,
                Email = model.Email,
                Password = model.Password
            };
        }

        private IActionResult InvalidLogin(LoginViewModel model)
        {
            ModelState.AddModelError(string.Empty, "Invalid credentials or blocked user.");
            return View(nameof(Login), model);
        }

        private IActionResult RegistrationFailed(RegisterViewModel model, string error)
        {
            ModelState.AddModelError(string.Empty, error);
            return View(nameof(Register), model);
        }

        private async Task SignInAsync(User user)
        {
            var principal = CreatePrincipal(user);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

        private static ClaimsPrincipal CreatePrincipal(User user)
        {
            var identity = new ClaimsIdentity(CreateClaims(user), CookieAuthenticationDefaults.AuthenticationScheme);

            return new ClaimsPrincipal(identity);
        }

        private static IReadOnlyList<Claim> CreateClaims(User user)
        {
            return
            [
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Name),
                new(ClaimTypes.Email, user.Email)
            ];
        }
    }
}
