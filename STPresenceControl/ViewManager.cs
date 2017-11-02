using STPresenceControl.Common;
using STPresenceControl.DataProviders;
using STPresenceControl.Models;
using STPresenceControl.Notification;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace STPresenceControl
{
    public class ViewManager
    {
        private System.ComponentModel.IContainer _components;
        private System.Windows.Forms.NotifyIcon _notifyIcon;
        private INotfication _notification;
        private IDataProvider _dataProvider;
        private DispatcherTimer _leftTimeTimer;
        private List<PresenceControlEntry> _presenceControlEntries;
        private double _leftMins;
        private DispatcherTimer _refreshData;
        public ViewManager()
        {
            _components = new System.ComponentModel.Container();
            _notifyIcon = new System.Windows.Forms.NotifyIcon(_components)
            {
                ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(),
                Icon = Properties.Resources.AppIcon,
                Text = String.Format("Faltan: {0}", new TimeSpan(0, Convert.ToInt32(_leftMins), 0).ToString()),
                Visible = true,
            };
            _notification = new NotifyIconBallonTip(_notifyIcon);

            _notification.Show("Iniciando...", "Control de presencia", Enums.NotificationTypeEnum.Info);

            _leftTimeTimer = new DispatcherTimer(
                   new TimeSpan(0, 1, 0),
                   DispatcherPriority.Normal,
                   (sender, e) =>
                   {
                       RefreshNotifyIcon();
                       _leftMins--;
                   },
                   Dispatcher.CurrentDispatcher);

            _dataProvider = new InfinityZucchetti();

            _refreshData = new DispatcherTimer(
                   new TimeSpan(0, 30, 0),
                   DispatcherPriority.Normal,
                   (sender, e) =>
                   {
                       GetPrensenceControlEntries();
                   },
                   Dispatcher.CurrentDispatcher);

            GetPrensenceControlEntries();
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
            var leftTimeSpan = new TimeSpan(0, Convert.ToInt32(_leftMins), 0);

            var colorIconText = Color.Green;
            var showIconAsText = false;
            var toolTipText = "Faltan {0}";

            if (_leftMins >= 0 && _leftMins < 60)
            {
                colorIconText = Color.Green;
                showIconAsText = true;
            }
            else if (_leftMins < 0)
            {
                toolTipText = "{0} de más";
                colorIconText = Color.Red;
                showIconAsText = true;
            }

            _notifyIcon.Text = String.Format(toolTipText, leftTimeSpan.ToString(@"hh\:mm"));

            if (showIconAsText)
            {
                _notifyIcon.Icon = Icons.CreateTextIcon(leftTimeSpan.ToString(@"mm"), colorIconText);
            }

            if (leftTimeSpan.ToString(@"hh\:mm") == "00:00")
            {
                _notification.Show("Ha terminado tu joranada laboral.", "Control de presencia", Enums.NotificationTypeEnum.Info);
            }
        }
    }
}
