using STPresenceControl.Common;
using STPresenceControl.DataProviders;
using STPresenceControl.Models;
using STPresenceControl.Notification;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace STPresenceControl
{
    public class ViewManager
    {
        private double _leftMins;
        private NotifyIcon _notifyIcon;
        private INotfication _notification;
        private IDataProvider _dataProvider = new InfinityZucchetti();
        private DispatcherTimer _refreshData;
        private DispatcherTimer _leftTimeTimer;
        private List<PresenceControlEntry> _presenceControlEntries;

        public ViewManager()
        {
            _notifyIcon = new NotifyIcon(new Container())
            {
                ContextMenuStrip = new ContextMenuStrip(),
                Icon = Properties.Resources.AppIcon,
                Text = String.Format("Faltan: {0}", new TimeSpan(0, Convert.ToInt32(_leftMins), 0).ToString()),
                Visible = true,
                ContextMenu = GenerateContextMenu()
            };
            _notification = new NotifyIconBallonTip(_notifyIcon);
            _notification.Show("Iniciando...", "Control de presencia", Enums.NotificationTypeEnum.Info);
            _leftTimeTimer = new DispatcherTimer(new TimeSpan(0, 1, 0), DispatcherPriority.Normal, (sender, e) =>
                  {
                      RefreshNotifyIcon();
                      _leftMins--;
                  },
                   Dispatcher.CurrentDispatcher);

            _refreshData = new DispatcherTimer(new TimeSpan(0, 30, 0), DispatcherPriority.Normal, (sender, e) => GetPrensenceControlEntries(), Dispatcher.CurrentDispatcher);

            GetPrensenceControlEntries();
        }

        private ContextMenu GenerateContextMenu()
        {
            var contextMenu = new ContextMenu();
            //contextMenu.MenuItems.Add("Info", ExecuteShowInfo);
            //contextMenu.MenuItems.Add("-");
            contextMenu.MenuItems.Add("Salir", ExecuteExit);
            return contextMenu;
        }

        //private void ExecuteShowInfo(object sender, EventArgs e)
        //{
        //    MessageBox.Show("Test");
        //}

        private void ExecuteExit(object sender, EventArgs e)
        {
            App.Current.Shutdown();
        }

        private void GetPrensenceControlEntries()
        {
            Task.Run(async () =>
            {
                await _dataProvider.LoginAsync(ConfigurationManager.AppSettings[App.CN_UserName], ConfigurationManager.AppSettings[App.CN_Pwd]);
                _presenceControlEntries = await _dataProvider.GetPrensenceControlEntriesAsync(DateTime.Today);
                _leftMins = PresenceControlEntriesHelper.GetLeftTimeMinutes(_presenceControlEntries);
                RefreshNotifyIcon();
#if DEBUG
                _notification.Show("Actualizas entradas y salidas.", "Control de presencia", Enums.NotificationTypeEnum.Info);
#endif
            });
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
    }
}
