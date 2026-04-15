namespace Task4.Web.ViewModels
{
    public sealed class UserRowViewModel
    {
        public Guid Id { get; init; }

        public string Name { get; init; } = string.Empty;

        public string Email { get; init; } = string.Empty;

        public string Status { get; init; } = string.Empty;

        public DateTime? LastLoginAtUtc { get; init; }
    }
}
