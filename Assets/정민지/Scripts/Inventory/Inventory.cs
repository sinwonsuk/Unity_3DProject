using static UnityEditor.Progress;
using UnityEngine;
using TMPro;

public class Inventory : MonoBehaviour
{
    public int slotCount = 15;
    public InventorySlot[] slots;
    public int gold = 100;
    public TMP_Text goldText;
    public InventoryUI inventoryUI;

    private void Awake()
    {
        slots = new InventorySlot[slotCount];
        for (int i = 0; i < slotCount; i++)
            slots[i] = new InventorySlot();
    }

    public bool AddItem(ItemData item)
    {
        if (item.isStackable)
        {
            foreach (var slot in slots)
            {
                if (!slot.IsEmpty && slot.item == item)
                {
                    slot.quantity++;
                    inventoryUI.UpdateUI();
                    return true;
                }
            }
        }

        foreach (var slot in slots)
        {
            if (slot.IsEmpty)
            {
                slot.item = item;
                slot.quantity = 1;
                slot.item.isStackable = true;
                inventoryUI.UpdateUI();
                return true;
            }
        }

        return false;
    }

    public void SellItem(ItemData item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            var slot = slots[i];
            if (!slot.IsEmpty && slot.item == item)
            {
                slot.quantity--;
                if (slot.quantity <= 0)
                    slot.Clear();

                gold += item.price;
                goldText.text = gold.ToString();
                inventoryUI.UpdateUI();
                break;
            }
        }
    }

    public bool HasItem(ItemData item, int count = 1)
    {
        int total = 0;
        foreach (var slot in slots)
        {
            if (!slot.IsEmpty && slot.item == item)
            {
                total += slot.quantity;
                if (total >= count)
                    return true;
            }
        }
        return false;
    }
}
