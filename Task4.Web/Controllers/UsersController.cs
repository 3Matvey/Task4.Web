using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Task4.Web.Services;
using Task4.Web.ViewModels;

namespace Task4.Web.Controllers;

[Authorize]
public sealed class UsersController(
    UserService userService,
    CurrentUserAccessor currentUserAccessor) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var users = await userService.GetUsersAsync(cancellationToken);

        return View(new UsersIndexViewModel
        {
            Users = users
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Block(BulkActionRequest request, CancellationToken cancellationToken)
    {
        await userService.BlockAsync(request.SelectedUserIds, cancellationToken);

        if (currentUserAccessor.IsCurrentUserSelected(User, request.SelectedUserIds))
            return await LogoutCurrentUserAsync();

        TempData["StatusMessage"] = "Selected users were blocked.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unblock(BulkActionRequest request, CancellationToken cancellationToken)
    {
        await userService.UnblockAsync(request.SelectedUserIds, cancellationToken);

        TempData["StatusMessage"] = "Selected users were unblocked.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(BulkActionRequest request, CancellationToken cancellationToken)
    {
        await userService.DeleteAsync(request.SelectedUserIds, cancellationToken);

        if (currentUserAccessor.IsCurrentUserSelected(User, request.SelectedUserIds))
            return await LogoutCurrentUserAsync();

        TempData["StatusMessage"] = "Selected users were deleted.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUnverified(BulkActionRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserAccessor.GetUserId(User);

        var deletedCurrentUser = await userService.DeleteUnverifiedAsync(request.SelectedUserIds, currentUserId, cancellationToken);

        if (deletedCurrentUser)
            return await LogoutCurrentUserAsync();

        TempData["StatusMessage"] = "Selected unverified users were deleted.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<IActionResult> LogoutCurrentUserAsync()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        TempData["StatusMessage"] = "Your account is no longer available.";
        return RedirectToAction("Login", "Auth");
    }
}