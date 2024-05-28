using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Net;
using System.Globalization;

public class Slot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title, content;
    [SerializeField] private Button[] purchaseButtons;
    private WaitForSecondsRealtime delay = new WaitForSecondsRealtime(1);
    private Coroutine timer = null;
    public void Initialize(Item item)
    {
        title.text = item.Name;
        if (item is TimeLimitedItem) SetTimer(((TimeLimitedItem)item).Limit);
        int index = 0;
        foreach(Currency currency in System.Enum.GetValues(typeof(Currency))) 
        {
            var purchaseMethod = GetMethod(currency);
            if (index < purchaseButtons.Length)
            {
                purchaseButtons[index++].onClick.AddListener(()=>PurchaseItem(purchaseMethod, item));
            }
        }
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
            return DateTime.Now;
        }
    }

    private void SetTimer(DateTime limit)
    {
        timer = StartCoroutine(UpdateTimer(limit));
    }

    private IEnumerator UpdateTimer(DateTime limit)
    {
        TimeSpan timeRemaining;
        do {
            timeRemaining = limit - GetCurrentDateTime();
            content.text = $"{timeRemaining.Hours:D2} : {timeRemaining.Minutes:D2} : {timeRemaining.Seconds:D2}";
            yield return delay;
        } while (timeRemaining > TimeSpan.Zero);
        content.text = "Expired";
        LockItem();
    }

    private void PurchaseItem(IPurchaseMethod purchaseMethod, Item item) 
    {
        item.Purchase(purchaseMethod);
        if (timer != null) StopCoroutine(timer);
        content.text = "Purchased";
        LockItem();
    }

    private void LockItem()
    {
        foreach (Button button in purchaseButtons) button.interactable = false;
    }

    IPurchaseMethod GetMethod(Currency currency) 
    {
        return currency switch
        {
            Currency.InGame => new InGameCurrencyPurchase(),
            Currency.Other => new OtherCurrencyPurchase(),
            _ => throw new ArgumentOutOfRangeException(nameof(currency), currency, null)
        };
    }

}
