namespace Task4.Web.Services
{
    public sealed class EmailConfirmationResult
    {
        private EmailConfirmationResult(bool succeeded, string error)
        {
            Succeeded = succeeded;
            Error = error;
        }

        public bool Succeeded { get; }

        public string Error { get; } = string.Empty;

        public static EmailConfirmationResult Success()
            => new(true, string.Empty);

        public static EmailConfirmationResult Fail(string error)
            => new(false, error);
    }
}
