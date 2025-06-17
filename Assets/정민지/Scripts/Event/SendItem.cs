using UnityEngine;

public class SendItem : IEvent
{
    public ItemData item;
    
    public SendItem(ItemData item)
    {
        this.item = item;
    }
}
