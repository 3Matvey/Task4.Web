using Microsoft.EntityFrameworkCore;
using Task4.Web.Data;
using Task4.Web.Models;
using Task4.Web.ViewModels;

namespace Task4.Web.Services;

public sealed class UserService(AppDbContext dbContext)
{
    public async Task<IReadOnlyList<UserRowViewModel>> GetUsersAsync(
        CancellationToken cancellationToken)
    {
        return await dbContext.Users
            .OrderByDescending(x => x.LastLoginAtUtc ?? x.CreatedAtUtc)
            .Select(x => new UserRowViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Email = x.Email,
                Status = x.Status.ToString(),
                LastLoginAtUtc = x.LastLoginAtUtc
            })
            .ToListAsync(cancellationToken);
    }

    public async Task BlockAsync(
        IReadOnlyList<Guid> ids,
        CancellationToken cancellationToken)
    {
        var users = await LoadUsersAsync(ids, cancellationToken);

        foreach (var user in users)
            user.Block();

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UnblockAsync(
        IReadOnlyList<Guid> ids,
        CancellationToken cancellationToken)
    {
        var users = await LoadUsersAsync(ids, cancellationToken);

        foreach (var user in users)
            user.Unblock();

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        IReadOnlyList<Guid> ids,
        CancellationToken cancellationToken)
    {
        var users = await LoadUsersAsync(ids, cancellationToken);

        dbContext.Users.RemoveRange(users);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteUnverifiedAsync(
        IReadOnlyList<Guid> ids,
        Guid? currentUserId,
        CancellationToken cancellationToken)
    {
        var users = await LoadUsersAsync(ids, cancellationToken);
        var toDelete = GetUnverifiedUsers(users);
        var deletedCurrentUser = IsDeletedCurrentUser(toDelete, currentUserId);

        dbContext.Users.RemoveRange(toDelete);
        await dbContext.SaveChangesAsync(cancellationToken);

        return deletedCurrentUser;
    }

    private static List<User> GetUnverifiedUsers(IEnumerable<User> users)
    {
        return users
            .Where(x => !x.IsEmailConfirmed)
            .ToList();
    }

    private static bool IsDeletedCurrentUser(
        IEnumerable<User> users,
        Guid? currentUserId)
    {
        return currentUserId.HasValue
            && users.Any(x => x.Id == currentUserId.Value);
    }

    private async Task<List<User>> LoadUsersAsync(
        IReadOnlyList<Guid> ids,
        CancellationToken cancellationToken)
    {
        return await dbContext.Users
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }
}