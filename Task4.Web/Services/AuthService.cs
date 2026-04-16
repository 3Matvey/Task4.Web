using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Task4.Web.Data;
using Task4.Web.Models;

namespace Task4.Web.Services
{
    public sealed class AuthService(
        AppDbContext dbContext,
        PasswordHasher<User> passwordHasher,
        EmailConfirmationService emailConfirmationService,
        TimeProvider timeProvider)
    {
        public async Task<AuthResult> RegisterAsync(
            RegisterUserCommand command,
            CancellationToken cancellationToken)
        {
            if (await EmailExistsAsync(command.Email, cancellationToken))
                return AuthResult.Fail("A user with this e-mail already exists.");

            var user = CreateUser(command);
            SetPasswordHash(user, command.Password);

            dbContext.Users.Add(user);

            var result = await SaveUserAsync(cancellationToken);

            if (!result.Succeeded)
                return result;

            await CreateAndSendConfirmationAsync(user, cancellationToken);

            return AuthResult.Success();
        }

        public async Task<User?> FindForLoginAsync(
            LoginCommand command,
            CancellationToken cancellationToken)
        {
            var user = await FindByEmailAsync(command.Email, cancellationToken);

            if (user is null)
                return null;

            if (!VerifyPassword(user, command.Password))
                return null;

            return user.CanLogin ? user : null;
        }

        public async Task RecordLoginAsync(
            User user,
            CancellationToken cancellationToken)
        {
            user.RecordLogin(GetUtcNow());
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task CreateAndSendConfirmationAsync(
            User user,
            CancellationToken cancellationToken)
        {
            await emailConfirmationService.CreateAndSendAsync(
                user,
                cancellationToken);
        }

        private async Task<bool> EmailExistsAsync(
            string email,
            CancellationToken cancellationToken)
        {
            var normalizedEmail = NormalizeEmail(email);

            return await dbContext.Users
                .AnyAsync(x => x.Email == normalizedEmail, cancellationToken);
        }

        private User CreateUser(RegisterUserCommand command)
        {
            return User.Create(
                command.Name,
                command.Email,
                string.Empty,
                GetUtcNow());
        }

        private void SetPasswordHash(User user, string password)
        {
            user.PasswordHash = passwordHasher.HashPassword(user, password);
        }

        private async Task<AuthResult> SaveUserAsync(
            CancellationToken cancellationToken)
        {
            try
            {
                await dbContext.SaveChangesAsync(cancellationToken);
                return AuthResult.Success();
            }
            catch (DbUpdateException ex) when (UserDbErrors.IsDuplicateEmail(ex))
            {
                return AuthResult.Fail("A user with this e-mail already exists.");
            }
        }

        private async Task<User?> FindByEmailAsync(
            string email,
            CancellationToken cancellationToken)
        {
            var normalizedEmail = NormalizeEmail(email);

            return await dbContext.Users
                .SingleOrDefaultAsync(x => x.Email == normalizedEmail, cancellationToken);
        }

        private PasswordVerificationResult VerifyPasswordResult(
            User user,
            string password)
        {
            return passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                password);
        }

        private bool VerifyPassword(User user, string password)
        {
            return VerifyPasswordResult(user, password)
                != PasswordVerificationResult.Failed;
        }

        private DateTime GetUtcNow()
        {
            return timeProvider.GetUtcNow().UtcDateTime;
        }

        private static string NormalizeEmail(string email)
        {
            return email.Trim().ToLowerInvariant();
        }
    }
}