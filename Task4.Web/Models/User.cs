namespace Task4.Web.Models
{
    public sealed class User
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; } = string.Empty;

        public string Email { get; private set; } = string.Empty;

        public string PasswordHash { get; internal set; } = string.Empty;

        public UserStatus Status { get; private set; }

        public DateTime CreatedAtUtc { get; private set; }

        public DateTime? LastLoginAtUtc { get; private set; }

        public DateTime? EmailConfirmedAtUtc { get; private set; }

        public bool CanLogin => Status != UserStatus.Blocked;

        public bool IsEmailConfirmed => EmailConfirmedAtUtc.HasValue;

        private User() { }

        private User(
            string name,
            string email,
            string passwordHash,
            DateTime createdAtUtc)
        {
            Name = NormalizeName(name);
            Email = NormalizeEmail(email);
            PasswordHash = passwordHash;
            CreatedAtUtc = createdAtUtc;
            Status = UserStatus.Unverified;
        }

        public static User Create(
            string name,
            string email,
            string passwordHash,
            DateTime createdAtUtc)
        {
            return new User(name, email, passwordHash, createdAtUtc);
        }

        public void ConfirmEmail(DateTime confirmedAtUtc)
        {
            EmailConfirmedAtUtc = confirmedAtUtc;

            if (Status == UserStatus.Unverified)
                Status = UserStatus.Active;
        }

        public void Block() => 
            Status = UserStatus.Blocked;

        public void Unblock()
        {
            if (Status != UserStatus.Blocked)
                return;

            Status = EmailConfirmedAtUtc.HasValue
                ? UserStatus.Active
                : UserStatus.Unverified;
        }

        public void RecordLogin(DateTime loginAtUtc) => 
            LastLoginAtUtc = loginAtUtc;

        private static string NormalizeName(string name)
        {
            return name.Trim();
        }

        private static string NormalizeEmail(string email)
        {
            return email.Trim().ToLowerInvariant();
        }
    }
}
