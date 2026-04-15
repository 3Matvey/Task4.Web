namespace Task4.Web.Services
{
    public sealed class AuthResult
    {
        private AuthResult(bool succeeded, string error)
        {
            Succeeded = succeeded;
            Error = error;
        }

        public bool Succeeded { get; }

        public string Error { get; } = string.Empty;

        public static AuthResult Success() =>
            new(true, string.Empty);

        public static AuthResult Fail(string error) =>
            new(false, error);
    }
}
