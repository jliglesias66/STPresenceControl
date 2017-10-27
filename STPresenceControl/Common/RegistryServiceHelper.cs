using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STPresenceControl.Common
{
    public class RegistryServiceHelper
    {
        const string CN_RegistryKeyPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";

        public static string AppName
        {
            get { return Path.GetFileNameWithoutExtension(ProcessName); }
        }

        public static string ProcessName
        {
            get { return Process.GetCurrentProcess().MainModule.FileName; }
        }

        public static void AddToStartup()
        {
            GetRegistryKey().SetValue(AppName, ProcessName);
        }

        public static void RemoveFromStartup()
        {
            GetRegistryKey().DeleteValue(AppName);
        }

        public static bool IsOnStartup()
        {
            return GetRegistryKey().GetValue(AppName) != null;
        }

        private static RegistryKey GetRegistryKey()
        {
            return Registry.CurrentUser.OpenSubKey(CN_RegistryKeyPath, true);
        }
    }
}
