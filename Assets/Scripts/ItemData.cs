using System;
using System.Collections.Generic;

[Serializable]
public class ItemData
{
    public string Name;
    public string Type;
    public string Limit;
}

[Serializable]
public class ItemDataList
{
    public List<ItemData> Items;
}