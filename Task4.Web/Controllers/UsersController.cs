using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Task4.Web.Services;
using Task4.Web.ViewModels;

namespace Task4.Web.Controllers
{
    [Authorize]
    public sealed class UsersController(UserService userService) : Controller
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
        public async Task<IActionResult> Block(
            BulkActionRequest request,
            CancellationToken cancellationToken)
        {
            await userService.BlockAsync(request.SelectedUserIds, cancellationToken);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unblock(
            BulkActionRequest request,
            CancellationToken cancellationToken)
        {
            await userService.UnblockAsync(request.SelectedUserIds, cancellationToken);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(
            BulkActionRequest request,
            CancellationToken cancellationToken)
        {
            await userService.DeleteAsync(request.SelectedUserIds, cancellationToken);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUnverified(
            BulkActionRequest request,
            CancellationToken cancellationToken)
        {
            await userService.DeleteUnverifiedAsync(request.SelectedUserIds, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
    }
}