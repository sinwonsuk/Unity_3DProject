using Fusion;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform slotParent;

    public ItemData basicSword;
    public ItemData basicBow;
    public ItemData basicHerberd;
    public ItemData basicFire;
    public ItemData basicIce;
    public ItemData basicLightning;


    public GameObject bigInventoryPanel;
    public GameObject combi;
    [SerializeField] private BigInventoryUI bigInventoryUI;
    [SerializeField] private float scrollCooldown = 0.2f; // 휠 입력 간 최소 간격 (초)
    [SerializeField] private float lastScrollTime = 0f;

    public int rowSize = 5;
    public int maxRow = 3;

    private Inventory inventory;
    private List<InventorySlotUI> slotUIs = new List<InventorySlotUI>();
    private int currentPage = 0;
    private int selectedIndex = 0;
    private bool isActive;
    private bool canSee;

    //public static InventoryUI Instance { get; private set; }

    //private void Awake()
    //{
    //    Instance = this;
    //}

    //public bool IsInventoryOpen()
    //{
    //    return bigInventoryPanel.activeSelf;
    //}


    private void Start()
    {
        inventory = GetComponent<Inventory>();
        InitSlots();
        inventory.AddItem(basicSword);
        inventory.AddItem(basicBow);
        inventory.AddItem(basicHerberd);
        inventory.AddItem(basicFire);
        inventory.AddItem(basicIce);
        inventory.AddItem(basicLightning);

        UpdateUI();
        isActive = false;
        canSee = false;
        SelectFirstWeaponSlot();
    }

    public void SelectFirstWeaponSlot()
    {
        for (int i = 0; i < inventory.slots.Length; i++)
        {
            var slot = inventory.slots[i];
            if (!slot.IsEmpty && slot.item != null)
            {
                OnSlotClicked(i);
                Debug.Log($"게임 시작 시 자동으로 무기 슬롯 {i} 선택됨: {slot.item.itemName}");
                break;
            }
        }
    }

    public void OnSlotClicked(int index)
    {
        int page = index / rowSize;
        int slotInRow = index % rowSize;

        currentPage = page;
        selectedIndex = slotInRow;

        UpdateUI();

        if (slotInRow < slotUIs.Count)
            slotUIs[slotInRow].ForceSelect();
    }

    void InitSlots()
    {
        for (int i = 0; i < rowSize; i++)
        {
            var slotGO = Instantiate(slotPrefab, slotParent);
            var slotUI = slotGO.GetComponent<InventorySlotUI>();
            slotUIs.Add(slotUI); //슬롯 배치
        }
    }

    public void UpdateUI() //선택된 아이템 업뎃
    {
        int startIndex = currentPage * rowSize;
        for (int i = 0; i < slotUIs.Count; i++)
        {
            int slotIndex = startIndex + i;
            if (slotIndex < inventory.slots.Length)
                slotUIs[i].SetSlot(inventory.slots[slotIndex], i == selectedIndex);
            else
                slotUIs[i].SetSlot(new InventorySlot(), false);
        }
    }

    public void IsOpenInven()
    {
        if (bigInventoryPanel.activeSelf || combi.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (!bigInventoryPanel.activeSelf && !combi.activeSelf)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {

        if(!combi.activeSelf && Input.GetKeyDown(KeyCode.I))
        {
            isActive = bigInventoryPanel.activeSelf;
            bigInventoryPanel.SetActive(!isActive); // 토글 방식

            if(bigInventoryPanel.activeSelf)
            {
                EventBus<showCursor>.Raise(new showCursor(true));
            }
            else
            {
                EventBus<showCursor>.Raise(new showCursor(false));
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            int prevIndex = currentPage * rowSize + selectedIndex;
            currentPage = (currentPage + 1) % maxRow;

            // 같은 열 유지한 채 다음 줄로 이동
            int newIndex = currentPage * rowSize + (prevIndex % rowSize);
            selectedIndex = newIndex % rowSize;

            if (newIndex < inventory.slots.Length)
            {
                OnSlotClicked(newIndex); //  이것만 해주면 자동 선택 + 무기 변경까지 OK
                bigInventoryUI.SetSelectedIndexInBigUI(newIndex);
                Debug.Log($"[TAB] {newIndex}번 슬롯 선택됨: {inventory.slots[newIndex].item?.itemName}");
            }
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            bool wasCombiOpen = combi.activeSelf;

            if (!wasCombiOpen)
            {
                // 조합창이 열리는 경우
                combi.SetActive(true);
                EventBus<showCursor>.Raise(new showCursor(true));

                if (!bigInventoryPanel.activeSelf)
                {
                    bigInventoryPanel.SetActive(true);
                }
            }
            else
            {
                // 조합창이 닫히는 경우, 인벤토리도 같이 닫음
                combi.SetActive(false);
                bigInventoryPanel.SetActive(false);
                EventBus<showCursor>.Raise(new showCursor(false));
            }
        }
            
        float scrollDir = Input.GetAxis("Mouse ScrollWheel");

        if (bigInventoryPanel.activeSelf == false && scrollDir != 0 && Time.time - lastScrollTime >= scrollCooldown)
        {
            if (scrollDir > 0)
                selectedIndex = (selectedIndex - 1 + rowSize) % rowSize;
            else if (scrollDir < 0)
                selectedIndex = (selectedIndex + 1) % rowSize;

            UpdateUI();
            lastScrollTime = Time.time;
        }
    }


    public ItemData GetSelectedItem() //선택된 아이템 가져오기
    {
        int index = currentPage * rowSize + selectedIndex;
        if (index < inventory.slots.Length)
            return inventory.slots[index].item;
        return null;
    }

    public void SetSelectedIndex(int index)
    {
        currentPage = index / rowSize;
        selectedIndex = index % rowSize;
        UpdateUI();
    }
}
