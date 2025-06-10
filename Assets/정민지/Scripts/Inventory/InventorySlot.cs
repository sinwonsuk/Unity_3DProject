using static UnityEditor.Progress;

[System.Serializable]
public class InventorySlot
{
    public ItemData item;
    public int quantity; //������ ����

    public bool IsEmpty => item == null || quantity <= 0;

    public void Clear()
    {
        if (item != null)
            item.isStackable = false; // �� ���� ��ǻ� ���ʿ��� ���� ����

        item = null;
        quantity = 0;
    }
}

