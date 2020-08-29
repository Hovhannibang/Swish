using Unity.Notifications.Android;
using UnityEngine;

public class NotificationController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var channel = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Default Channel",
            Importance = Importance.Default,
            Description = "Generic notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }

    public void setupDailyNotification()
    {
        var notification = new AndroidNotification
        {
            Text = "Claim your daily 50 Gems!",
            FireTime = System.DateTime.Now.AddDays(1)
        };
        AndroidNotificationCenter.SendNotification(notification, "channel_id");
    }

    private void OnApplicationFocus(bool focus)
    {
        AndroidNotificationCenter.CancelAllDisplayedNotifications();
    }
}
