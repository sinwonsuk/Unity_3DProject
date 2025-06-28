using UnityEngine;

public class SendSlot :IEvent
{
    public InventorySlot potion;
    public SendSlot(InventorySlot potion)
    {
        this.potion = potion;
    }
}
