using System.Security.Claims;

namespace Task4.Web.Services
{
    public sealed class CurrentUserAccessor
    {
        public Guid? GetUserId(ClaimsPrincipal principal)
        {
            var value = principal.FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(value, out var userId)
                ? userId
                : null;
        }

        public bool IsCurrentUserSelected(
            ClaimsPrincipal principal,
            IReadOnlyCollection<Guid> selectedUserIds)
        {
            var userId = GetUserId(principal);

            return userId.HasValue && selectedUserIds.Contains(userId.Value);
        }
    }
}
