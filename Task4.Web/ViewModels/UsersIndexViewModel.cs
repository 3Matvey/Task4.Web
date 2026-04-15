namespace Task4.Web.ViewModels
{
    public sealed class UsersIndexViewModel
    {
        public IReadOnlyList<UserRowViewModel> Users { get; init; } = [];
    }
}
