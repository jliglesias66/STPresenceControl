using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using STPresenceControl.Enums;

namespace STPresenceControl.Notification
{
    public class NotifyIconBallonTip : INotfication 
    {
        public readonly NotifyIcon NotifyIcon;
        public NotifyIconBallonTip(NotifyIcon notifyIcon)
        {
            NotifyIcon = notifyIcon;
        }

        public void Show(string message)
        {
            Show(message, null);
        }

        public void Show(string message, string title)
        {
            Show(message, title, NotificationTypeEnum.None);
        }

        public void Show(string message, string title, NotificationTypeEnum notificationType)
        {
            Show(message, title, notificationType, 3000);
        }

        public void Show(string message, string title, NotificationTypeEnum notificationType, int showMilliseconds)
        {
            NotifyIcon.BalloonTipText = message;
            NotifyIcon.BalloonTipTitle = title;
            NotifyIcon.BalloonTipIcon = (ToolTipIcon)Enum.Parse(typeof(ToolTipIcon), notificationType.ToString());
            NotifyIcon.ShowBalloonTip(showMilliseconds);
        }
    }
}
