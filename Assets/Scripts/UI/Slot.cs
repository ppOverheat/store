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
        Debug.Log(item.IsAvailable);
        title.text = item.Name;
        if (item is TimeLimitedItem && !item.IsAvailable)
            SetTimer(((TimeLimitedItem)item).Limit);
        
        for (int i = 0; i < PurchaseMethodRegistry.purchaseMethods.Count; i++)
        {
            var button = Instantiate(buttonPrefab, buttonsGrid).GetComponent<Button>();
            var method = PurchaseMethodRegistry.GetPurchaseMethod(i);
            button.GetComponentInChildren<TextMeshProUGUI>().text = method.GetType().ToString();
            button.onClick.AddListener(()=>PurchaseItem(method, item));
            purchaseButtons.Add(button);
        }
        if (item.IsAvailable) 
        {
            content.text = "Purchased";
            LockItem();
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
        TimeSpan timeRemaining = limit - DateTime.UtcNow;
        while (timeRemaining > TimeSpan.Zero) 
        {
            timeRemaining = limit - DateTime.UtcNow;//GetCurrentDateTime();
            content.text = $"{(int)timeRemaining.TotalHours} : {timeRemaining.Minutes:D2} : {timeRemaining.Seconds:D2}";
            yield return delay;
        }
        content.text = "Expired";
        LockItem();
    }

    private void PurchaseItem(IPurchaseMethod purchaseMethod, Item item) 
    {
        Item old = item;
        item.Purchase(purchaseMethod);
        if (timer != null) StopCoroutine(timer);
        content.text = "Purchased";
        LockItem();
        SaveSystem.Save(old, item);
    }

    private void LockItem()
    {
        foreach (Button button in purchaseButtons) button.interactable = false;
    }
}
