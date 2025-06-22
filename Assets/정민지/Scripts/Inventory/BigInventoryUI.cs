using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BigInventoryUI : MonoBehaviour
{
    public GameObject slotPrefab;
    public InventoryUI inventoryOnoff;
    public Transform slotParent;
    public Transform dragLayer; // �巡�׿� �ֻ��� UI ���̾�, �ν����Ϳ��� ����

    [SerializeField] private Inventory inventory;
    private List<InventorySlotUI> slotUIs = new List<InventorySlotUI>();
    private InventorySlot[] slots; // ���� �κ� ���� �迭

    public DraggedIcon draggedIcon; // �ν����� ����

    public int selectedIndex = -1;

    void Start()
    {
        InitSlots();
        slots = inventory.slots; // ���� �迭 �ʱ�ȭ
        UpdateUI();

    }

    void InitSlots()
    {
        for (int i = 0; i < inventory.slots.Length; i++)
        {
            var slotGO = Instantiate(slotPrefab, slotParent);
            var slotUI = slotGO.GetComponent<InventorySlotUI>();
            slotUI.Initialize(this, i,draggedIcon); // ���Կ� �ε����� owner ����
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
        inventoryOnoff.SetSelectedIndex(selectedIndex); // ���� �κ��丮 UI���� �ݿ�
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
