using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;

public static class SaveSystem
{
    private static string save_path = $"{Application.persistentDataPath}/save.json";
    public static List<Item> Load()
    {
        List<Item> items = new List<Item>();
        try 
        {
            if (File.Exists(save_path))
            { 
                ItemDataList data = JsonConvert.DeserializeObject<ItemDataList>(File.ReadAllText(save_path));
                foreach (ItemData itemData in data.Items)
                    items.Add(ItemCreator.Create(itemData.Type, itemData.Arguments));
            }
        }
        catch (System.Exception)
        {
        }
        return items;
    }

    public static List<Item> Load(string jsonString)
    {
        List<Item> items = new List<Item>();
        try 
        {
            ItemDataList data = JsonConvert.DeserializeObject<ItemDataList>(jsonString);
            foreach (ItemData itemData in data.Items)
                items.Add(ItemCreator.Create(itemData.Type, itemData.Arguments));
        }
        catch (System.Exception)
        {
        }
        return items;
    }

    public static void Save(List<Item> data)
    {
        ItemDataList dataList = new ItemDataList{Items = new List<ItemData>()};
        foreach (Item item in data)
        {
            dataList.Items.Add(new ItemData{
                Type = $"{item.GetType()}",
                Arguments = JsonConvert.SerializeObject(item)
            });
        }
        File.WriteAllText(save_path, JsonConvert.SerializeObject(dataList, Formatting.Indented));
        Debug.Log($"save path:{save_path}");
    }
}
