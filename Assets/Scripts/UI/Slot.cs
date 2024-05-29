using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class Slot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title, content;
    [SerializeField] private Transform buttonsGrid;
    [SerializeField] private GameObject buttonPrefab;
    private List<Button> purchaseButtons = new List<Button>();
    private ItemTimer itemTimer;
    private List<Item> itemList;
    public void Initialize(Item item)
    {
        title.text = item.Name;
        if (item is TimeLimitedItem && !item.IsAvailable)
        {
            itemTimer = new ItemTimer(content, ((TimeLimitedItem)item).Limit, this);
            itemTimer.OnTimerExpired += () => OnTimerExpired(item);
            itemTimer.StartTimer();
        }
        
        for (int i = 0; i < PurchaseMethodRegistry.purchaseMethods.Count; i++)
        {
            var button = Instantiate(buttonPrefab, buttonsGrid).GetComponent<Button>();
            var method = PurchaseMethodRegistry.GetPurchaseMethod(i);
            button.GetComponentInChildren<TextMeshProUGUI>().text = (Enum.IsDefined(typeof(Currency), i) && item.Cost!=null && item.Cost.ContainsKey((Currency)i))?$"{item.Cost[(Currency)i]} {(Currency)i}":$"{method.GetType()}";
            button.onClick.AddListener(()=>PurchaseItem(method, item));
            purchaseButtons.Add(button);
        }
        if (item.IsAvailable) 
        {
            content.text = "Purchased";
            LockItem();
        }
    }

    private void PurchaseItem(IPurchaseMethod purchaseMethod, Item item) 
    {
        Item old = item;
        item.Purchase(purchaseMethod);
        itemTimer?.StopTimer();
        content.text = "Purchased";
        LockItem();
        SaveSystem.Save(old, item);
    }
    private void OnTimerExpired(Item item)
    {
        item.IsAvailable = false;
        LockItem();
        SaveSystem.Save(itemList);
    }
    public void LockItem()
    {
        foreach (Button button in purchaseButtons) button.interactable = false;
    }
    
}
