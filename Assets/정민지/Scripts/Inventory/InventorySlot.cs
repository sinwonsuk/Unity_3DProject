using static UnityEditor.Progress;

[System.Serializable]
public class InventorySlot
{
    public ItemData item;
    public int quantity; //아이템 개수

    public bool IsEmpty => item == null || quantity <= 0;

    public void Clear()
    {
        item = null;
        quantity = 0;
    }
}

