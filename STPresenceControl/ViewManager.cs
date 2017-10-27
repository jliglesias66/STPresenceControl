using STPresenceControl.Common;
using STPresenceControl.DataProviders;
using STPresenceControl.Models;
using STPresenceControl.Notification;
using STPresenceControl.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;

namespace STPresenceControl
{
    public class ViewManager
    {
        #region Const

        const string CN_Info = "Configuración";
        const string CN_Refresh = "Refrescar";
        const string CN_Auto = "Autoarranque";
        const string CN_Exit = "Salir";
        const string CN_Separator = "-";

        #endregion

        #region Fields

        private double _leftMins;
        private readonly NotifyIcon _notifyIcon;
        private readonly INotfication _notification;
        private readonly IDataProvider _dataProvider = new InfinityZucchetti();
        private DispatcherTimer _refreshData;
        private DispatcherTimer _leftTimeTimer;

        #endregion

        #region Properties

        private readonly List<PresenceControlEntry> _presenceControlEntries = new List<PresenceControlEntry>();
        public List<PresenceControlEntry> PresenceControlEntries { get { return _presenceControlEntries; } }

        #endregion

        #region Public

        private static ViewManager _instance;

        public static void Start()
        {
            _instance = new ViewManager();
        }

        public static List<PresenceControlEntry> GetPresenceEntries()
        {
            if (_instance == null)
                Start();
            return _instance.PresenceControlEntries;
        }

        #endregion

        #region Context Menu

        private ContextMenu GenerateContextMenu()
        {
            var contextMenu = new ContextMenu();
            contextMenu.MenuItems.Add(CN_Info, ExecuteShowConfig);
            contextMenu.MenuItems.Add(CN_Refresh, ExecuteRefresh);
            contextMenu.MenuItems.Add(CN_Auto, ExecuteChangeStartup);
            contextMenu.MenuItems.Add(CN_Separator);
            contextMenu.MenuItems.Add(CN_Exit, ExecuteExit);
            return contextMenu;
        }

        private void ExecuteChangeStartup(object sender, EventArgs e)
        {
            if (RegistryServiceHelper.IsOnStartup())
                RegistryServiceHelper.RemoveFromStartup();
            else
                RegistryServiceHelper.AddToStartup();
            CheckContextMenuState();
        }

        private void ExecuteRefresh(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void CheckContextMenuState()
        {
            //¿?
            //_notifyIcon.ContextMenu.MenuItems[CN_Auto].Checked = RegistryServiceHelper.IsOnStartup();
            _notifyIcon.ContextMenu.MenuItems[2].Checked = RegistryServiceHelper.IsOnStartup();
        }

        private void ExecuteShowConfig(object sender, EventArgs e)
        {
            var window = new Window();
            window.Content = new MainViewModel();
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.ShowDialog();
        }

        private void ExecuteExit(object sender, EventArgs e)
        {
            App.Current.Shutdown();
        }

        #endregion

        #region Private

        private ViewManager()
        {
            _notifyIcon = new NotifyIcon(new Container())
            {
                ContextMenuStrip = new ContextMenuStrip(),
                Icon = Properties.Resources.AppIcon,
                Text = String.Format("Faltan: {0}", new TimeSpan(0, Convert.ToInt32(_leftMins), 0).ToString()),
                Visible = true,
                ContextMenu = GenerateContextMenu()
            };
            CheckContextMenuState();
            _notification = new NotifyIconBallonTip(_notifyIcon);
            _notification.Show("Iniciando...", "Control de presencia", Enums.NotificationTypeEnum.Info);
            _refreshData = new DispatcherTimer(new TimeSpan(0, 30, 0), DispatcherPriority.Normal, (sender, e) => GetPrensenceControlEntries(), Dispatcher.CurrentDispatcher);
            _leftTimeTimer = new DispatcherTimer(new TimeSpan(0, 1, 0), DispatcherPriority.Normal, (sender, e) =>
                  {
                      RefreshNotifyIcon();
                      _leftMins--;
                  },
                   Dispatcher.CurrentDispatcher);
            GetPrensenceControlEntries();
        }

        private async void GetPrensenceControlEntries()
        {
            try
            {
                PresenceControlEntries.Clear();
                await _dataProvider.LoginAsync(ConfigurationManager.AppSettings[App.CN_UserName], ConfigurationManager.AppSettings[App.CN_Pwd]);
                PresenceControlEntries.AddRange(await _dataProvider.GetPrensenceControlEntriesAsync(DateTime.Today));
                _leftMins = PresenceControlEntriesHelper.GetLeftTimeMinutes(_presenceControlEntries);
                _notification.Show("Actualizadas entradas y salidas.", "Control de presencia", Enums.NotificationTypeEnum.Info);
                RefreshNotifyIcon();
            }
            catch
            {
                //TODO - Sistema de log
            }
        }

        private void RefreshNotifyIcon()
        {
            if (_presenceControlEntries.Count == 0)
                return;
            var leftTimeSpan = new TimeSpan(0, Convert.ToInt32(_leftMins), 0);
            var colorIconText = Color.Green;

            if (leftTimeSpan.TotalHours >= 1)
                _notifyIcon.Icon = Icons.CreateTextIcon(((int)leftTimeSpan.TotalHours).ToString() + "h", Color.Red);
            else
                _notifyIcon.Icon = Icons.CreateTextIcon(leftTimeSpan.TotalMinutes.ToString(), Color.Green);
            _notifyIcon.Text = String.Format("Tiempo restante {0}", leftTimeSpan.ToString(@"hh\:mm"));
            if (leftTimeSpan.TotalMinutes < 1)
                _notification.Show("Ha terminado tu jornada laboral.", "Control de presencia", Enums.NotificationTypeEnum.Info);
        }

        #endregion
    }
}
