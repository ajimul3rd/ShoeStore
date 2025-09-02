namespace ShoeStore.Servicess
{
    public class AppState
    {
        // ✅ Shared properties
        public string UserName { get; private set; } = "Guest";
        public string Password { get; private set; } = "Admin";

        // ✅ Change notification event
        public event Action? OnChange;

        // ✅ Method to update values and notify subscribers
        public void SetUser(string userName, string password)
        {
            UserName = userName;
            Password = password;
            NotifyStateChanged();
        }

        // ✅ Notify subscribers safely
        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}