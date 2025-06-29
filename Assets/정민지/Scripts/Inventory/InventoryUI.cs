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
    [SerializeField] private float scrollCooldown = 0.2f; // �� �Է� �� �ּ� ���� (��)
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
                Debug.Log($"���� ���� �� �ڵ����� ���� ���� {i} ���õ�: {slot.item.itemName}");
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
            bigInventoryPanel.SetActive(!isActive); // ��� ���

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

            // ���� �� ������ ä ���� �ٷ� �̵�
            int newIndex = currentPage * rowSize + (prevIndex % rowSize);
            selectedIndex = newIndex % rowSize;

            if (newIndex < inventory.slots.Length)
            {
                OnSlotClicked(newIndex); //  �̰͸� ���ָ� �ڵ� ���� + ���� ������� OK
                bigInventoryUI.SetSelectedIndexInBigUI(newIndex);
                Debug.Log($"[TAB] {newIndex}�� ���� ���õ�: {inventory.slots[newIndex].item?.itemName}");
            }
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            bool wasCombiOpen = combi.activeSelf;

            if (!wasCombiOpen)
            {
                // ����â�� ������ ���
                combi.SetActive(true);
                EventBus<showCursor>.Raise(new showCursor(true));

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
