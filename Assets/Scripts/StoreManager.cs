using System;
using System.Collections.Generic;
using UnityEngine;

public class StoreManager : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private TextAsset jsonFile;
    private List<Item> items = new List<Item>();

    void Start()
    {
        LoadItemsFromJson();
        DisplayItems();
    }
    private void LoadItemsFromJson()
    {
        if (jsonFile != null)
        {
            ItemDataList itemDataList = JsonUtility.FromJson<ItemDataList>(jsonFile.text);
            foreach (ItemData itemData in itemDataList.Items)
            {
                Item item = CreateItemFromData(itemData);
                if (item != null)
                {
                    items.Add(item);
                }
            }
        }
    }
    private Item CreateItemFromData(ItemData data)
    {
        switch (data.Type)
        {
            case "Concrete":
                return new ConcreteItem { Name = data.Name };
            case "TimeLimited":
                DateTime limit;
                if (DateTime.TryParse(data.Limit, out limit))
                {
                    return new TimeLimitedItem { Name = data.Name, Limit = limit };
                }
                break;
        }
        return null;
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
