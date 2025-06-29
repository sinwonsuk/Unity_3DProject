using UnityEngine;

public struct showCursor : IEvent
{
    public bool canSeeInventory;

    public showCursor(bool canSeeInventory)
    {
        this.canSeeInventory = canSeeInventory;
    }
}
