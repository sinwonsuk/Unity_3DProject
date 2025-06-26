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

    public int rowSize = 5;
    public int maxRow = 3;

    private Inventory inventory;
    private List<InventorySlotUI> slotUIs = new List<InventorySlotUI>();
    private int currentPage = 0;
    private int selectedIndex = 0;
    private bool isActive;
    private bool canSee;

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

    private void SelectFirstWeaponSlot()
    {
        for (int i = 0; i < inventory.slots.Length; i++)
        {
            var slot = inventory.slots[i];
            if (!slot.IsEmpty && slot.item!=null)
            {
                OnSlotClicked(i);
                Debug.Log($"���� ���� �� �ڵ����� ���� ���� {i} ���õ�: {slot.item.itemName}");
                break;
            }
        }
    }

    public void OnSlotClicked(int index)
    {
        selectedIndex = index;
        UpdateUI();
    }

    void InitSlots()
    {
        for (int i = 0; i < rowSize; i++)
        {
            var slotGO = Instantiate(slotPrefab, slotParent);
            var slotUI = slotGO.GetComponent<InventorySlotUI>();
            slotUIs.Add(slotUI); //���� ��ġ
        }
    }

    public void UpdateUI() //���õ� ������ ����
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

        if(!combi.activeSelf && Input.GetKeyDown(KeyCode.I))
        {
            isActive = bigInventoryPanel.activeSelf;
            bigInventoryPanel.SetActive(!isActive); // ��� ���
        }
            
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            int index = currentPage * rowSize + selectedIndex;
            bigInventoryUI.SetSelectedIndexInBigUI(index); // ���� �ε��� ����ȭ

            currentPage = (currentPage + 1) % maxRow;
            selectedIndex = 0;
            UpdateUI();
        }

        if(Input.GetKeyDown(KeyCode.C))
        {
            bool wasCombiOpen = combi.activeSelf;

            if (!wasCombiOpen)
            {
                // ����â�� ������ ���
                combi.SetActive(true);

                if (!bigInventoryPanel.activeSelf)
                {
                    bigInventoryPanel.SetActive(true);
                }
            }
            else
            {
                // ����â�� ������ ���, �κ��丮�� ���� ����
                combi.SetActive(false);
                bigInventoryPanel.SetActive(false);
            }
        }
            
        float scrollDir = Input.GetAxis("Mouse ScrollWheel");

        if (bigInventoryPanel.activeSelf == false && scrollDir != 0)
        {
            if (scrollDir > 0)
                selectedIndex = (selectedIndex - 1 + rowSize) % rowSize;
            else if (scrollDir < 0)
                selectedIndex = (selectedIndex + 1) % rowSize;

            UpdateUI();
        }
    }


    public ItemData GetSelectedItem() //���õ� ������ ��������
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
