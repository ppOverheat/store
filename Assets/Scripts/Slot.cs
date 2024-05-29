using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Net;
using System.Globalization;
using System.Collections.Generic;

public class Slot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title, content;
    [SerializeField] private Transform buttonsGrid;
    [SerializeField] private GameObject buttonPrefab;
    private List<Button> purchaseButtons = new List<Button>();
    private WaitForSecondsRealtime delay = new WaitForSecondsRealtime(1);
    private Coroutine timer = null;
    public void Initialize(Item item)
    {
        title.text = item.Name;
        if (item is TimeLimitedItem)
        {
            SetTimer(((TimeLimitedItem)item).Limit);
        }
        for (int i = 0; i < PurchaseMethodRegistry.purchaseMethods.Count; i++)
        {
            var button = Instantiate(buttonPrefab, buttonsGrid).GetComponent<Button>();
            var method = PurchaseMethodRegistry.GetPurchaseMethod(i);
            button.onClick.AddListener(()=>PurchaseItem(method, item));
            purchaseButtons.Add(button);
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
            return DateTime.UtcNow;
        }
    }

    private void SetTimer(DateTime limit)
    {
        timer = StartCoroutine(UpdateTimer(limit));
    }

    private IEnumerator UpdateTimer(DateTime limit)
    {
        Debug.Log(limit);
        TimeSpan timeRemaining = limit - DateTime.UtcNow;
        Debug.Log($"{limit}-{DateTime.UtcNow}={(int)timeRemaining.TotalHours} : {timeRemaining.Minutes:D2} : {timeRemaining.Seconds:D2}");

        do {
            timeRemaining = limit - DateTime.UtcNow;//GetCurrentDateTime();
            content.text = $"{(int)timeRemaining.TotalHours} : {timeRemaining.Minutes:D2} : {timeRemaining.Seconds:D2}";
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
}
