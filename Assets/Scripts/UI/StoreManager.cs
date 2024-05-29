using System.Collections.Generic;
using UnityEngine;

public class StoreManager : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private TextAsset jsonFile;

    void Awake()
    {
        PurchaseMethodRegistry.RegisterPurchaseMethod(typeof(InGameCurrencyPurchase));
        PurchaseMethodRegistry.RegisterPurchaseMethod(typeof(OtherCurrencyPurchase));
    }

    void Start()
    {
        DisplayItems();
    }

    private void DisplayItems()
    {
        List<Item> items = SaveSystem.Load();
        if (items.Count==0) items = SaveSystem.Load(jsonFile.text);
        foreach (Item item in items)
        {
            var slot = Instantiate(slotPrefab, transform).GetComponent<Slot>();
            slot.Initialize(item);
        }
    }
}
