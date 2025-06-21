using UnityEngine;

public class RequestToOpenOrCloseInventory : IEvent
{
    public bool isShopOpen;

    public RequestToOpenOrCloseInventory(bool isShopOpen)
    {
        this.isShopOpen = isShopOpen;
    }
}
