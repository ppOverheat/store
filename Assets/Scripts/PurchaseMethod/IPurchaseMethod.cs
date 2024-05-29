using UnityEngine;

public interface IPurchaseMethod
{
    void Purchase(Item item);
}

public class InGameCurrencyPurchase : IPurchaseMethod
{
    public void Purchase(Item item)
    {
        Debug.Log($"{item.Name} purchased by in-game currency");
    }
}

public class OtherCurrencyPurchase : IPurchaseMethod
{
    public void Purchase(Item item)
    {
        Debug.Log($"{item.Name} purchased by other currency");
    }
}

public enum Currency {
    InGame,
    Other
}