using static UnityEditor.Progress;

[System.Serializable]
public class InventorySlot
{
    public ItemData item;
    public int quantity; //아이템 개수

    public bool IsEmpty => item == null || quantity <= 0;

    public void Clear()
    {
        if (item != null)
            item.isStackable = false; // 이 줄은 사실상 불필요할 수도 있음

        item = null;
        quantity = 0;
    }
}

