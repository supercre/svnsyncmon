using System.Globalization;
using System.Resources;

namespace SVNSyncMon
{
    public static class Localization
    {
        private static readonly ResourceManager ResourceManager = new ResourceManager("SVNSyncMon.Resources.Strings", typeof(Localization).Assembly);
        private static AppSettings settings = AppSettings.Load();

        public static string CurrentLanguage
        {
            get => settings.Language;
            set
            {
                if (settings.Language != value)
                {
                    settings.Language = value;
                    settings.Save();
                    CultureInfo.CurrentUICulture = new CultureInfo(value);
                }
            }
        }

        public static string GetString(string name)
        {
            return ResourceManager.GetString(name, new CultureInfo(CurrentLanguage)) ?? name;
        }

        public static string GetString(string name, params object[] args)
        {
            string? format = ResourceManager.GetString(name, new CultureInfo(CurrentLanguage));
            return format != null ? string.Format(format, args) : name;
        }
    }
} 