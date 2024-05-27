using System;
using UnityEngine;

public abstract class Item
{
    public string Name { get; set; }
    public bool IsAvailable { get; protected set; }
    public abstract void Purchase(IPurchaseMethod purchaseMethod);
}

public class TimeLimitedItem : Item
{
    public DateTime Limit { get; set; }
    public override void Purchase(IPurchaseMethod purchaseMethod)
    {
        if (DateTime.Now <= Limit)
        {
            purchaseMethod.Purchase(this);
            IsAvailable = true;
        }
        else
        {
            Debug.Log($"{Name} is no longer available for purchase.");
        }
    }
}

public class ConcreteItem : Item
{
    public override void Purchase(IPurchaseMethod purchaseMethod)
    {
        purchaseMethod.Purchase(this);
        IsAvailable = true;
    }
}