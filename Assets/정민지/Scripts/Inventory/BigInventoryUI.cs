using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BigInventoryUI : MonoBehaviour
{
    public GameObject slotPrefab;
    public InventoryUI inventoryOnoff;
    public Transform slotParent;

    [SerializeField] private Inventory inventory;
    private List<InventorySlotUI> slotUIs = new List<InventorySlotUI>();

    private int selectedIndex = -1;

    void Start()
    {
        InitSlots();
        UpdateUI();
    }

    void InitSlots()
    {
        for (int i = 0; i < inventory.slots.Length; i++)
        {
            var slotGO = Instantiate(slotPrefab, slotParent);
            var slotUI = slotGO.GetComponent<InventorySlotUI>();
            slotUI.Initialize(this, i); // ���Կ� �ε����� owner ����
            slotUIs.Add(slotUI);
        }
    }

    public void OnSlotClicked(int index) //���� Ŭ�� ��
    {
        selectedIndex = index;
        UpdateUI();
        // ���� �κ��丮���� ���� �ε��� ����
        inventoryOnoff.SetSelectedIndex(index);
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
            var slot = inventory.slots[i];
            slotUIs[i].SetSlot(slot, i == selectedIndex);
        }
    }


    public ItemData GetBigSelectedItem()
    {
        if (selectedIndex >= 0 && selectedIndex < inventory.slots.Length)
        {
            return inventory.slots[selectedIndex].item;
        }
        return null;
    }

}
