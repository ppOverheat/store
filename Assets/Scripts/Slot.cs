using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title, content;
    [SerializeField] private Button[] purchaseButtons;

    public void Initialize(Item item)
    {
        title.text = item.Name;
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

    private void PurchaseItem(IPurchaseMethod purchaseMethod, Item item) 
    {
        item.Purchase(purchaseMethod);
        content.text = "purchased";
        foreach (Button button in purchaseButtons)
        {
            button.interactable = false;
        }
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
