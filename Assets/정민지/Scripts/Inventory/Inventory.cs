using static UnityEditor.Progress;
using UnityEngine;
using TMPro;

public class Inventory : MonoBehaviour
{

    public int slotCount = 15; //�κ��丮 ĭ ����
    public InventorySlot[] slots; //�κ��丮 ���� ĭ
    public InventoryUI inventoryUI; //�κ��丮UI
    private int gold; //���� ������Ʈ�� �޾ƿ��� ���� ��
    public int basicGold; //�⺻���� �����ϴ� ��
    public GoldUI goldUI; //��� UI
    public BigInventoryUI bigInventoryUI; //�κ��丮 ��ü ȭ��
    [SerializeField] private ItemManager itemManager;

    private void Awake()
    {
       
        slots = new InventorySlot[slotCount]; //ĭ ������ �´� �κ��丮 ���� �迭 ����

        for (int i = 0; i < slotCount; i++) //���� �迭�� ���Ե� �ֱ�
            slots[i] = new InventorySlot();

        gold = basicGold; //�� �⺻ ����
        EventBus<Gold>.Raise(new Gold(basicGold)); //�⺻���� ������ �� ���ֱ�

    }


    //��UI ����
    private void OnEnable()
    {
        EventBus<Gold>.OnEvent += UpdateGold;
    }

    //��UI ���� ����
    private void OnDisable()
    {
        EventBus<Gold>.OnEvent -= UpdateGold;
    }

    //�� �ǽð� ������Ʈ
    private void UpdateGold(Gold _gold)
    {
        gold = _gold.currentGold;
    }

    //public void AddItemById(int itemId)
    //{
    //    var itemData = itemManager.GetItem(itemId);
    //    if (itemData != null)
    //    {
    //        AddItem(itemData);
    //    }
    //    else
    //    {
    //        Debug.LogWarning($"Item ID {itemId}�� �ش��ϴ� �������� ã�� �� �����ϴ�.");
    //    }
    //}

    public void UpdateAllInventoryUI()
    {
        inventoryUI?.UpdateUI();
        bigInventoryUI?.UpdateUI();
    }

    //������ �߰�
    public void AddItem(ItemData item)
    {
        foreach (var slot in slots)
        {
            if(slot.item.itemType == ItemType.Potion)
            {
                if (!slot.IsEmpty && slot.item == item)
                {
                    slot.quantity++;
                    UpdateAllInventoryUI();
                    return;
                }
                else if (slot.IsEmpty)
                {
                    slot.item = item;
                    slot.quantity = 1;
                    UpdateAllInventoryUI();
                    return;
                }
            }
            else
            {
                if(slot.IsEmpty)
                {
                    slot.item = item;
                    slot.quantity = 1;
                    UpdateAllInventoryUI();
                    return;
                }
            }
              
        }      
    }

    ////������ ����
    //public void BuyItemById(int itemId)
    //{
    //    var item = itemManager.GetItem(itemId);
    //    if (item == null)
    //    {
    //        Debug.LogWarning($"������ ID {itemId}�� �������� �ʽ��ϴ�.");
    //        return;
    //    }

    //    if (gold >= item.price)
    //    {
    //        goldUI.SubtractGold(item.price);
    //        AddItem(item);
    //        Debug.Log("������ ���� �Ϸ�");
    //    }
    //}

    //������ ����
    public void SellItem(ItemData item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            var slot = slots[i];
            if (!slot.IsEmpty && slot.item == item)
            {
                slot.quantity--;

                if (slot.quantity <= 0)
                {
                    slot.Clear();
                    UpdateAllInventoryUI();
                }
                UpdateAllInventoryUI();
                break;
            }
        }
    }

    //public bool HasItem(ItemData item, int count = 1)
    //{
    //    int total = 0;
    //    foreach (var slot in slots)
    //    {
    //        if (!slot.IsEmpty && slot.item == item)
    //        {
    //            total += slot.quantity;
    //            if (total >= count)
    //                return true;
    //        }
    //    }
    //    return false;
    //}
}
