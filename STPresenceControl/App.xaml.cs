using System.Windows;
using System.Windows.Forms;

namespace STPresenceControl
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        #region Const

        public const string CN_UserName = "UserName";
        public const string CN_Pwd = "Pwd";

        #endregion

        protected override void OnStartup(StartupEventArgs e)
        {
            // Use the assembly GUID as the name of the mutex which we use to detect if an application instance is already running
            bool createdNew = false;
            string mutexName = System.Reflection.Assembly.GetExecutingAssembly().GetType().GUID.ToString();
            using (System.Threading.Mutex mutex = new System.Threading.Mutex(false, mutexName, out createdNew))
            {
                if (!createdNew)
                {
                    // Only allow one instance
                    return;
                }
                StartUpSTApplication();

                base.OnStartup(e);
            }
        }

        private void StartUpSTApplication()
        {
            var viewManager = new ViewManager();
        }
    }
}
