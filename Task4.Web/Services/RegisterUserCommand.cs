namespace Task4.Web.Services
{
    public sealed class RegisterUserCommand
    {
        public string Name { get; init; } = string.Empty;

        public string Email { get; init; } = string.Empty;

        public string Password { get; init; } = string.Empty;
    }
}
