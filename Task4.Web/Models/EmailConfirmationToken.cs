namespace Task4.Web.Models
{
    public sealed class EmailConfirmationToken
    {
        public Guid Id { get; private set; }

        public Guid UserId { get; private set; }

        public string Token { get; private set; } = string.Empty;

        public DateTime CreatedAtUtc { get; private set; }

        public DateTime ExpiresAtUtc { get; private set; }

        public DateTime? UsedAtUtc { get; private set; }

        public User User { get; private set; } = null!;

        public bool IsUsed => UsedAtUtc.HasValue;

        private EmailConfirmationToken() { }

        private EmailConfirmationToken(Guid userId, string token, DateTime createdAtUtc, DateTime expiresAtUtc)
        {
            UserId = userId;
            Token = token;
            CreatedAtUtc = createdAtUtc;
            ExpiresAtUtc = expiresAtUtc;
        }

        public static EmailConfirmationToken Create(Guid userId, DateTime nowUtc, TimeSpan lifetime)
        {
            return new EmailConfirmationToken(userId, GenerateToken(), nowUtc, nowUtc.Add(lifetime));
        }

        public bool IsExpired(DateTime nowUtc) =>
            nowUtc > ExpiresAtUtc;

        public bool CanBeUsed(DateTime nowUtc) =>
            !IsUsed && !IsExpired(nowUtc);

        public void MarkAsUsed(DateTime usedAtUtc)
        {
            if (IsUsed)
                return;

            UsedAtUtc = usedAtUtc;
        }

        private static string GenerateToken()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}