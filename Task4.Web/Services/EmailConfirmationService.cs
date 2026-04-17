using Microsoft.EntityFrameworkCore;
using Task4.Web.Data;
using Task4.Web.Models;

namespace Task4.Web.Services
{
    public sealed class EmailConfirmationService(
        AppDbContext dbContext,
        EmailService emailService,
        TimeProvider timeProvider)
    {
        public async Task CreateAndSendAsync(
            Guid userId,
            string email,
            string scheme,
            string host,
            CancellationToken cancellationToken)
        {
            var token = EmailConfirmationToken.Create(
                userId,
                GetUtcNow(),
                TimeSpan.FromHours(24));

            dbContext.EmailConfirmationTokens.Add(token);
            await dbContext.SaveChangesAsync(cancellationToken);

            var link = BuildEmailConfirmationLink(
                token.Token,
                scheme,
                host);

            await emailService.SendEmailAsync(
                email,
                "Confirm your e-mail",
                CreateBody(link),
                cancellationToken);
        }

        public async Task<EmailConfirmationResult> ConfirmAsync(
            string token,
            CancellationToken cancellationToken)
        {
            var entity = await FindTokenAsync(token, cancellationToken);

            if (entity is null)
                return EmailConfirmationResult.Fail("Confirmation token was not found.");

            if (!entity.CanBeUsed(GetUtcNow()))
                return EmailConfirmationResult.Fail("Confirmation token is invalid or expired.");

            entity.MarkAsUsed(GetUtcNow());
            entity.User.ConfirmEmail(GetUtcNow());

            await dbContext.SaveChangesAsync(cancellationToken);

            return EmailConfirmationResult.Success();
        }

        private async Task<EmailConfirmationToken?> FindTokenAsync(
            string token,
            CancellationToken cancellationToken)
        {
            return await dbContext.EmailConfirmationTokens
                .Include(x => x.User)
                .SingleOrDefaultAsync(x => x.Token == token, cancellationToken);
        }

        private static string CreateBody(string link)
        {
            return $"""
                <p>Please confirm your e-mail by clicking the link below.</p>
                <p><a href="{link}">Confirm e-mail</a></p>
                <p>{link}</p>
             """;
        }

        public string BuildEmailConfirmationLink(
            string token,
            string scheme,
            string host)
        {
            return $"{scheme}://{host}/Email/Confirm?token={Uri.EscapeDataString(token)}";
        }

        private DateTime GetUtcNow()
        {
            return timeProvider.GetUtcNow().UtcDateTime;
        }
    }
}