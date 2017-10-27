using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace STPresenceControl
{
    public partial class App : Application
    {
        #region Const

        public const string CN_UserName = "UserName";
        public const string CN_Pwd = "Pwd";

        #endregion

        protected override void OnStartup(StartupEventArgs e)
        {
            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Count() != 1)
                return;
            ViewManager.Start();
            base.OnStartup(e);
        }
    }
}
