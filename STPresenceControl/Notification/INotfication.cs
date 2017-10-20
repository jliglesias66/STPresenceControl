using STPresenceControl.Enums;

namespace STPresenceControl.Notification
{
    public interface INotfication
    {
        void Show(string message);
        void Show(string message, string title);
        void Show(string message, string title, NotificationTypeEnum notificationType);
        void Show(string message, string title, NotificationTypeEnum notificationType, int showMilliseconds);
    }
}
