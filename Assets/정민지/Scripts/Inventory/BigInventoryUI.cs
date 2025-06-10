using System.Collections.Generic;
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryOnoff.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    void InitSlots()
    {
        for (int i = 0; i < inventory.slots.Length; i++)
        {
            var slotGO = Instantiate(slotPrefab, slotParent);
            var slotUI = slotGO.GetComponent<InventorySlotUI>();
            slotUI.Initialize(this, i); // ½½·Ô¿¡ ÀÎµ¦½º¿Í owner Àü´Þ
            slotUIs.Add(slotUI);
        }
    }

    public void OnSlotClicked(int index) //½½·Ô Å¬¸¯ ½Ã
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

    public ItemData GetSelectedItem()
    {
        if (selectedIndex >= 0 && selectedIndex < inventory.slots.Length)
        {
            return inventory.slots[selectedIndex].item;
        }
        return null;
    }

    public void OnClickAndSellItem()
    {
        inventory.SellItem(GetSelectedItem());
        UpdateUI();
    }

    //private void OnEnable()
    //{
    //    UpdateUI();
    //}
}
