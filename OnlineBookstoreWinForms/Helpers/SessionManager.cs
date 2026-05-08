using OnlineBookstoreWinForms.Models;

namespace OnlineBookstoreWinForms.Helpers;

public static class SessionManager
{
    public static User? CurrentUser { get; private set; }
    public static bool IsLoggedIn => CurrentUser != null;
    public static bool IsAdmin => CurrentUser?.Role == Constants.Roles.Admin;

    public static void Login(User user) => CurrentUser = user;
    public static void Logout() => CurrentUser = null;
}
