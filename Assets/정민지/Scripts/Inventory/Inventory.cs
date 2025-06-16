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
        EventBus<BuyItemRequested>.OnEvent += BuyItem;
    }

    //��UI ���� ����
    private void OnDisable()
    {
        EventBus<Gold>.OnEvent -= UpdateGold;
        EventBus<BuyItemRequested>.OnEvent -= BuyItem;
    }

    //�� �ǽð� ������Ʈ
    private void UpdateGold(Gold _gold)
    {
        gold = _gold.currentGold;
    }

    private void BuyItem(BuyItemRequested newItem)
    {
        for (int i = 0; i < newItem.amount; i++)
        {
            AddItem(newItem.itemData);
        }
    }

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
            if (!slot.IsEmpty && slot.item == item && item.itemType == ItemType.Potion)
            {
                slot.quantity++;
                UpdateAllInventoryUI();
                return;
            }
            if (slot.IsEmpty)
            {
                slot.item = item;
                slot.quantity = 1;
                UpdateAllInventoryUI();
                return;
            }
        }

        Debug.Log("�κ��丮�� �� ������ �����ϴ�.");
    }

    //������ ����
    public void SellItem(ItemData item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            var slot = slots[i];
            if (!slot.IsEmpty && slot.item == item&&i==bigInventoryUI.selectedIndex)
            {
                EventBus<GetGold>.Raise(new GetGold(item.price));
                slot.quantity--;

                if (slot.quantity <= 0)
                {
                    slot.Clear();
                    UpdateAllInventoryUI();
                    break;
                }

                UpdateAllInventoryUI();
                break;
            }
        }
    }
}
