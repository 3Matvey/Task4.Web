namespace Task4.Web.Services
{
    public sealed class LinkBuilder(IHttpContextAccessor httpContextAccessor)
    {
        public string BuildEmailConfirmationLink(string token)
        {
            var request = httpContextAccessor.HttpContext!.Request;

            return $"{request.Scheme}://{request.Host}/Email/Confirm?token={token}";
        }
    }
}
