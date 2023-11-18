namespace Rocky.BLL.Constants;

public class AppSettings
{
    public static string LocalizationResourcesPath = "Localizations.Phrases";
    public static string LocalizationCulture = "en-US";
    public static bool isRightToLeft = false;
    public static int DateUTCAdjustment = +2;

    public static class UserSettings
    {
        public static int MaxFailedAccessAttempts = 5;
        public static int LockoutTimeSpanInMinutes = 5;
        public static string TokenBearerCookieName = "RockyAuth";
    }
}