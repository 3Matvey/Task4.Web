using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Task4.Web.Services;

namespace Task4.Web.Controllers
{
    [AllowAnonymous]
    public sealed class EmailController(
    EmailConfirmationService emailConfirmationService) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Confirm(
            string token,
            CancellationToken cancellationToken)
        {
            var result = await emailConfirmationService.ConfirmAsync(
                token,
                cancellationToken);

            if (!result.Succeeded)
                return BadRequest(result.Error);

            TempData["StatusMessage"] = "E-mail was confirmed successfully.";
            return RedirectToAction("Login", "Auth");
        }
    }
}
