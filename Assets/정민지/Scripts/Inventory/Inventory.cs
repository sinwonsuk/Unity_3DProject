using UnityEngine;
using TMPro;
using System.Linq;

public class Inventory : MonoBehaviour
{

    public int slotCount = 15;
    public InventorySlot[] slots; 
    public InventoryUI inventoryUI; 
    private int gold; 
    public int basicGold; 
    public GoldUI goldUI; 
    public BigInventoryUI bigInventoryUI; 
    [SerializeField] private ItemManager itemManager;


    private void Awake()
    {
       
        slots = new InventorySlot[slotCount]; 

        for (int i = 0; i < slotCount; i++) 
            slots[i] = new InventorySlot();

        gold = basicGold;
        EventBus<Gold>.Raise(new Gold(basicGold));

    }



    private void OnEnable()
    {
        EventBus<Gold>.OnEvent += UpdateGold;
        EventBus<BuyItemRequested>.OnEvent += BuyItem;
        EventBus<RequestItemToInventory>.OnEvent += GetItem;
    }


    private void OnDisable()
    {
        EventBus<Gold>.OnEvent -= UpdateGold;
        EventBus<BuyItemRequested>.OnEvent -= BuyItem;
        EventBus<RequestItemToInventory>.OnEvent -= GetItem;
    }


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

    private void GetItem(RequestItemToInventory evt)
    {
        AddItem(evt.itemData);
    }

    public void UpdateAllInventoryUI()
    {
        inventoryUI?.UpdateUI();
        bigInventoryUI?.UpdateUI();
    }

    
    public void AddItem(ItemData item)
    {
        if (item.itemType == ItemType.Potion)
        {
            
            foreach (var slot in slots)
            {
                if (!slot.IsEmpty && slot.item == item)
                {
                    slot.quantity++;
                    UpdateAllInventoryUI();
                    Debug.Log($"아이템 추가: {item.itemName}, 아이템 추가 개수: {slot.quantity}");
                    return;
                }
            }

            
            bool hasEmptySlot = slots.Any(slot => slot.IsEmpty);
            if (!hasEmptySlot) return;

            foreach (var slot in slots)
            {
                if (slot.IsEmpty)
                {
                    slot.item = item;
                    slot.quantity = 1;
                    UpdateAllInventoryUI();
                    Debug.Log($"아이템 추가: {item.itemName}");
                    return;
                }
            }
        }
        else
        {
            
            bool hasEmptySlot = slots.Any(slot => slot.IsEmpty);
            if (!hasEmptySlot) return;

            foreach (var slot in slots)
            {
                if (slot.IsEmpty)
                {
                    slot.item = item;
                    slot.quantity = 1;
                    UpdateAllInventoryUI();
                    Debug.Log($"아이템 추가: {item.itemName}");
                    return;
                }
            }
        }

        Debug.Log("아이템 추가 실패");
    }

    
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
