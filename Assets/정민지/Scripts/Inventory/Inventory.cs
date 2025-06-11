using static UnityEditor.Progress;
using UnityEngine;
using TMPro;

public class Inventory : MonoBehaviour
{

    public int slotCount = 15; //인벤토리 칸 개수
    public InventorySlot[] slots; //인벤토리 슬롯 칸
    public InventoryUI inventoryUI; //인벤토리UI
    private int gold; //돈의 업데이트를 받아오기 위한 돈
    public int basicGold; //기본으로 제공하는 돈
    public GoldUI goldUI; //골드 UI
    public BigInventoryUI bigInventoryUI; //인벤토리 전체 화면
    [SerializeField] private ItemManager itemManager;

    private void Awake()
    {
       
        slots = new InventorySlot[slotCount]; //칸 개수에 맞는 인벤토리 슬롯 배열 생성

        for (int i = 0; i < slotCount; i++) //슬롯 배열에 슬롯들 넣기
            slots[i] = new InventorySlot();

        gold = basicGold; //돈 기본 제공
        EventBus<Gold>.Raise(new Gold(basicGold)); //기본으로 제공한 돈 쏴주기

    }


    //돈UI 구독
    private void OnEnable()
    {
        EventBus<Gold>.OnEvent += UpdateGold;
    }

    //돈UI 구독 해제
    private void OnDisable()
    {
        EventBus<Gold>.OnEvent -= UpdateGold;
    }

    //돈 실시간 업데이트
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
    //        Debug.LogWarning($"Item ID {itemId}에 해당하는 아이템을 찾을 수 없습니다.");
    //    }
    //}

    public void UpdateAllInventoryUI()
    {
        inventoryUI?.UpdateUI();
        bigInventoryUI?.UpdateUI();
    }

    //아이템 추가
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

    ////아이템 구매
    //public void BuyItemById(int itemId)
    //{
    //    var item = itemManager.GetItem(itemId);
    //    if (item == null)
    //    {
    //        Debug.LogWarning($"아이템 ID {itemId}는 존재하지 않습니다.");
    //        return;
    //    }

    //    if (gold >= item.price)
    //    {
    //        goldUI.SubtractGold(item.price);
    //        AddItem(item);
    //        Debug.Log("아이템 구매 완료");
    //    }
    //}

    //아이템 삭제
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
