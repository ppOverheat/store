using System;
using System.Collections.Generic;
using UnityEngine;

public class StoreManager : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private TextAsset jsonFile;
    private List<Item> items = new List<Item>();

    void Awake()
    {
        PurchaseMethodRegistry.RegisterPurchaseMethod(typeof(InGameCurrencyPurchase));
        PurchaseMethodRegistry.RegisterPurchaseMethod(typeof(OtherCurrencyPurchase));
    }

    void Start()
    {
        LoadItems();
    }

    private void LoadItems()
    {
        items = SaveSystem.Load();
        if (items.Count==0) items = SaveSystem.Load(jsonFile.text);
        DisplayItems();
    }

    private void DisplayItems()
    {
        foreach (Item item in items)
        {
            var slot = Instantiate(slotPrefab, transform).GetComponent<Slot>();
            slot.Initialize(item);
        }
    }
}
