using UnityEngine;
using TMPro;
using System.Linq;

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
        EventBus<BuyItemRequested>.OnEvent += BuyItem;
        EventBus<RequestItemToInventory>.OnEvent += GetItem;
    }

    //돈UI 구독 해제
    private void OnDisable()
    {
        EventBus<Gold>.OnEvent -= UpdateGold;
        EventBus<BuyItemRequested>.OnEvent -= BuyItem;
        EventBus<RequestItemToInventory>.OnEvent -= GetItem;
    }

    //돈 실시간 업데이트
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

    //아이템 추가
    public void AddItem(ItemData item)
    {
        if (item.itemType == ItemType.Potion)
        {
            // 이미 같은 포션이 있으면 수량만 증가
            foreach (var slot in slots)
            {
                if (!slot.IsEmpty && slot.item == item)
                {
                    slot.quantity++;
                    UpdateAllInventoryUI();
                    Debug.Log($"포션 스택 증가: {item.itemName}, 현재 수량: {slot.quantity}");
                    return;
                }
            }

            // 같은 포션 없으면 빈 슬롯 있는지 체크 후 추가
            bool hasEmptySlot = slots.Any(slot => slot.IsEmpty);
            if (!hasEmptySlot) return;

            foreach (var slot in slots)
            {
                if (slot.IsEmpty)
                {
                    slot.item = item;
                    slot.quantity = 1;
                    UpdateAllInventoryUI();
                    Debug.Log($"빈 슬롯에 포션 추가: {item.itemName}");
                    return;
                }
            }
        }
        else
        {
            // 포션이 아닐 때는 무조건 빈 슬롯 필요
            bool hasEmptySlot = slots.Any(slot => slot.IsEmpty);
            if (!hasEmptySlot) return;

            foreach (var slot in slots)
            {
                if (slot.IsEmpty)
                {
                    slot.item = item;
                    slot.quantity = 1;
                    UpdateAllInventoryUI();
                    Debug.Log($"빈 슬롯에 아이템 추가: {item.itemName}");
                    return;
                }
            }
        }

        Debug.Log("아이템 추가 실패");
    }

    //아이템 삭제
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
