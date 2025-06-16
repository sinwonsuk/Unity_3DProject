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
        EventBus<BuyItemRequested>.OnEvent += BuyItem;
    }

    //돈UI 구독 해제
    private void OnDisable()
    {
        EventBus<Gold>.OnEvent -= UpdateGold;
        EventBus<BuyItemRequested>.OnEvent -= BuyItem;
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

        Debug.Log("인벤토리에 빈 슬롯이 없습니다.");
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
