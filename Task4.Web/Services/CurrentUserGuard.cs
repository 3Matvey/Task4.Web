using System.Security.Claims;
using Task4.Web.Data;
using Microsoft.EntityFrameworkCore;
using Task4.Web.Models;

namespace Task4.Web.Services
{
    public sealed class CurrentUserGuard(AppDbContext dbContext)
    {
        public async Task<bool> IsValidAsync(
            ClaimsPrincipal principal,
            CancellationToken cancellationToken)
        {
            var userId = GetUserId(principal);

            if (userId is null)
                return false;

            return await dbContext.Users
                .AnyAsync(
                    x => x.Id == userId && x.Status != UserStatus.Blocked,
                    cancellationToken);
        }

        private static Guid? GetUserId(ClaimsPrincipal principal)
        {
            var value = principal.FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(value, out var userId) 
                ? userId 
                : null;
        }
    }
}
