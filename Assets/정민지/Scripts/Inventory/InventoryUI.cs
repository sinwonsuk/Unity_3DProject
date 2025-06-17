using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform slotParent;
    public ItemData basicSword;
    public GameObject bigInventoryPanel;
    [SerializeField] private BigInventoryUI bigInventoryUI;

    public int rowSize = 5;
    public int maxRow = 3;
    public ItemData basicItem;

    private Inventory inventory;
    private List<InventorySlotUI> slotUIs = new List<InventorySlotUI>();
    private int currentPage = 0;
    private int selectedIndex = 0;

    private void Start()
    {
        inventory = GetComponent<Inventory>();
        InitSlots();
        inventory.AddItem(basicItem);
        UpdateUI();
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            bool isActive = bigInventoryPanel.activeSelf;
            bigInventoryPanel.SetActive(!isActive); // 토글 방식

            int index = currentPage * rowSize + selectedIndex;
            bigInventoryUI.SetSelectedIndexInBigUI(index); // 선택 인덱스 동기화
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            currentPage = (currentPage + 1) % maxRow;
            selectedIndex = 0;
            UpdateUI();
        }

        if (bigInventoryPanel.activeSelf == false)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (scroll > 0f)
                selectedIndex = (selectedIndex - 1 + rowSize) % rowSize;
            else if (scroll < 0f)
                selectedIndex = (selectedIndex + 1) % rowSize;

            UpdateUI();
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
