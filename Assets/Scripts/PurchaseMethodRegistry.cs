using System;
using System.Collections.Generic;
public static class PurchaseMethodRegistry
{
    public static List<Type> purchaseMethods = new List<Type>();

    public static void RegisterPurchaseMethod(Type type)
    {
        if (!typeof(IPurchaseMethod).IsAssignableFrom(type))
        {
            throw new ArgumentException("Invalid type", nameof(type));
        }
        purchaseMethods.Add(type);
    }

    public static IPurchaseMethod GetPurchaseMethod(int index)
    {
        if (index < purchaseMethods.Count && index >= 0)
        {
            return (IPurchaseMethod)Activator.CreateInstance(purchaseMethods[index]);
        }
        throw new KeyNotFoundException($"Purchase method not found for index: {index}");
    }
}
