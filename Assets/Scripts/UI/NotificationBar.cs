using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;

public class NotificationBar : MonoBehaviour
{
    public TextMeshProUGUI notifCount;

    void Start()
    {
        UIManager.instance.SendNotification += Notification;
    }

    private void Update()
    {
        notifCount.text = EmailManager.instance.emails.Where(x => x.tag != Email.EmailTags.Spam).Count().ToString();
    }

    public void Notification(string message, bool sound)
    {
        GetComponent<TextMeshProUGUI>().text = message;
        if (sound) GetComponent<AudioSource>().Play();
    }

    private void OnDestroy()
    {
        UIManager.instance.SendNotification -= Notification;
    }
}
