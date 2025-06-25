using Fusion;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.UI.GridLayoutGroup;

public class InventorySlotUI : NetworkBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Image iconImage;
    public TMP_Text quantityText;
    public Image highlightImage;

    private int index;
    private BigInventoryUI bigInventoryUI;

    private Transform originalParent;
    private DraggedIcon draggedIcon;
    private InventorySlot slot;
    private bool wasSelected = false; // ���� ���� ����


    public void SetSlot(InventorySlot slot, bool isSelected = false)
    {
        this.slot = slot;

        if (slot.IsEmpty)
        {
            iconImage.enabled = false;
            quantityText.text = "";
        }
        else
        {
            iconImage.enabled = true;
            iconImage.sprite = slot.item.itemIcon;
            quantityText.text = slot.quantity > 1 ? slot.quantity.ToString() : "";
        }

        // ���� ���� ����
        highlightImage.enabled = isSelected;

        // ���� ���� ���� ����
        if (wasSelected != isSelected)
        {
            wasSelected = isSelected;
            OnSelectionChanged(isSelected); // ���⼭ ���ϴ� �Լ� ����
        }
    }

    public void Initialize(BigInventoryUI ownerUI, int slotIndex, DraggedIcon icon)
    {
        bigInventoryUI = ownerUI;
        index = slotIndex;
        draggedIcon = icon;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        bigInventoryUI.OnSlotClicked(index);
        if (eventData.button == PointerEventData.InputButton.Right && slot.item != null)
        {
            EventBus<SendItem>.Raise(new SendItem(slot.item));
            slot.item = null;
            bigInventoryUI.UpdateSlotUI(index);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!iconImage.enabled) return;

        eventData.pointerDrag = gameObject;
        draggedIcon.gameObject.SetActive(true);
        draggedIcon.SetIcon(iconImage.sprite);
        draggedIcon.StartDrag(iconImage.sprite);
    }
    public void OnDrag(PointerEventData eventData)
    {
        // �巡�� �������� �ڵ����� ���콺�� ���� (Update���� ó����)
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        draggedIcon.gameObject.SetActive(false);
    }

    public void OnDrop(PointerEventData eventData)
    {
        InventorySlotUI draggedSlotUI = eventData.pointerDrag?.GetComponent<InventorySlotUI>();
        if (draggedSlotUI != null && draggedSlotUI != this)
        {
            bigInventoryUI.SwapSlots(draggedSlotUI.index, this.index);
        }
    }

    //[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    //public void RPC_ChangeWeapon(ItemState state, RpcInfo info = default)
    //{
    //    EventBus<WeaponChange>.Raise(new WeaponChange(state));
    //}


    private void OnSelectionChanged(bool isSelected)
    {
        if (isSelected)
        {
            Debug.Log($"[{index}] ������ ���õ�: {slot.item?.itemName}");

            if (slot.item == null)
            {
                //RPC_ChangeWeapon(ItemState.none);
                Debug.Log("���õ� �������� ����");
            }

            else if (slot.item.weaponType==WeaponType.Sword)
            {
                //RPC_ChangeWeapon(ItemState.none);

            }
            else if(slot.item.weaponType==WeaponType.Bow)
            {
                //RPC_ChangeWeapon(ItemState.Bow);

            }
            else if(slot.item.weaponType==WeaponType.Axe)
            {
               // RPC_ChangeWeapon(ItemState.Harberd);

            }
            else if(slot.item.potionType!=PotionType.NONE)
            {
              // RPC_ChangeWeapon(ItemState.none);

            }
            else if(slot.item.magicType==MagicType.Fire)
            {
                // RPC_ChangeWeapon(ItemState.FireMagic);
 
            }
            else if (slot.item.magicType == MagicType.Ice)
            {
                // RPC_ChangeWeapon(ItemState.IceMagic);

            }
            else if (slot.item.magicType == MagicType.Lightning)
            {
                // RPC_ChangeWeapon(ItemState.ElectricMagic);

            }

        }
        else
        {
            Debug.Log($"[{index}] ���� ���� ����");
        }
    }
}
