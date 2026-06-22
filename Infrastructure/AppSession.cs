using QLXeMay.Models;

namespace QLXeMay.Infrastructure
{
    internal static class AppSession
    {
        public static UserSession CurrentUser { get; private set; }

        public static bool IsAuthenticated => CurrentUser != null;

        public static void SignIn(UserSession session)
        {
            CurrentUser = session;
        }

        public static void SignOut()
        {
            CurrentUser = null;
        }
    }
}
