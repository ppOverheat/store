using System;
using Newtonsoft.Json;

public class ItemCreator
{
    public static Item Create(string className, string args)
    {
        Type type = Type.GetType(className);
        if (type == null)
            throw new ArgumentException($"Type {className} not found");
        Item instance = (Item)Activator.CreateInstance(type);
        JsonConvert.PopulateObject(args, instance);
        return instance;
    }
}
