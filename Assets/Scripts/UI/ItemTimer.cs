using System;
using System.Collections;
using UnityEngine;
using TMPro;
using System.Net;
using System.Globalization;

public class ItemTimer
{
    private TextMeshProUGUI content;
    private DateTime limit;
    private Coroutine timerCoroutine;
    private MonoBehaviour coroutineOwner;
    private WaitForSecondsRealtime delay = new WaitForSecondsRealtime(1);
    public event Action OnTimerExpired;
    private DateTime startTime;

    public ItemTimer(TextMeshProUGUI content, DateTime limit, MonoBehaviour coroutineOwner)
    {
        this.content = content;
        this.limit = limit;
        this.coroutineOwner = coroutineOwner;
    }

    public void StartTimer()
    {
        if (timerCoroutine != null)
            coroutineOwner.StopCoroutine(timerCoroutine);
        timerCoroutine = coroutineOwner.StartCoroutine(UpdateTimer());
    }

    public void StopTimer()
    {
        if (timerCoroutine != null)
        {
            coroutineOwner.StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }
    }
    private IEnumerator UpdateTimer()
    {
        TimeSpan timeRemaining = limit - DateTime.UtcNow;
        while (timeRemaining > TimeSpan.Zero)
        {
            content.text = $"{(int)timeRemaining.TotalHours} : {timeRemaining.Minutes:D2} : {timeRemaining.Seconds:D2}";
            yield return delay;
            timeRemaining = limit - DateTime.UtcNow;
        }
        content.text = "Expired";
        OnTimerExpired?.Invoke();
    }
    
    private DateTime GetCurrentDateTime()
    {
        try
        {
            using (var response = WebRequest.Create("http://www.google.com").GetResponse())
                return DateTime.ParseExact(response.Headers["date"],
                    "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                    CultureInfo.InvariantCulture.DateTimeFormat,
                    DateTimeStyles.AssumeUniversal);
        }
        catch (WebException)
        {
            return DateTime.UtcNow;
        }
    }
}
