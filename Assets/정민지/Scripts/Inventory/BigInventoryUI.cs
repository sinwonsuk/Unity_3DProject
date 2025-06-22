using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BigInventoryUI : MonoBehaviour
{
    public GameObject slotPrefab;
    public InventoryUI inventoryOnoff;
    public Transform slotParent;
    public Transform dragLayer; // 드래그용 최상위 UI 레이어, 인스펙터에서 지정

    [SerializeField] private Inventory inventory;
    private List<InventorySlotUI> slotUIs = new List<InventorySlotUI>();
    private InventorySlot[] slots; // 실제 인벤 슬롯 배열

    public DraggedIcon draggedIcon; // 인스펙터 연결

    public int selectedIndex = -1;

    void Start()
    {
        InitSlots();
        slots = inventory.slots; // 슬롯 배열 초기화
        UpdateUI();

    }

    void InitSlots()
    {
        for (int i = 0; i < inventory.slots.Length; i++)
        {
            var slotGO = Instantiate(slotPrefab, slotParent);
            var slotUI = slotGO.GetComponent<InventorySlotUI>();
            slotUI.Initialize(this, i,draggedIcon); // 슬롯에 인덱스와 owner 전달
            slotUIs.Add(slotUI);
        }
    }

    public void OpenOrClose(bool isOpen)
    {
        gameObject.SetActive(isOpen);
    }
    public Transform GetDragLayer()
    {
        return dragLayer;
    }

    public void OnSlotClicked(int index)
    {
        if (selectedIndex == index)
        {
            selectedIndex = -1;
        }
        else
        {
            selectedIndex = index;
        }

        UpdateUI();
        inventoryOnoff.SetSelectedIndex(selectedIndex); // 작은 인벤토리 UI에도 반영
    }


    public void SetSelectedIndexInBigUI(int index)
    {
        selectedIndex = index;
        UpdateUI();
    }

    public void UpdateUI()
    {
        for (int i = 0; i < slotUIs.Count; i++)
        {
            slotUIs[i].SetSlot(slots[i], i == selectedIndex);
        }
    }

    public ItemData GetBigSelectedItem()
    {
        if (selectedIndex >= 0 && selectedIndex < slots.Length)
        {
            return slots[selectedIndex].item;
        }
        return null;
    }

    public void SwapSlots(int fromIndex, int toIndex)
    {
        InventorySlot bowl = slots[fromIndex];
        slots[fromIndex] = slots[toIndex];
        slots[toIndex] = bowl;

        UpdateSlotUI(fromIndex);
        UpdateSlotUI(toIndex);
        inventoryOnoff.SetSelectedIndex(selectedIndex);
    }


    public void UpdateSlotUI(int index)
    {
        if (index < 0 || index >= slots.Length)
            return;

        slotUIs[index].SetSlot(slots[index], index == selectedIndex);
    }
}
