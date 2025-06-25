using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using static UnityEngine.UI.GridLayoutGroup;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Image iconImage;
    public TMP_Text quantityText;
    public Image highlightImage;

    private int index;
    private BigInventoryUI bigInventoryUI;

    private Transform originalParent;
    private DraggedIcon draggedIcon;
    private InventorySlot slot;
    private bool wasSelected = false; // 이전 선택 상태


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

        // 선택 여부 갱신
        highlightImage.enabled = isSelected;

        // 선택 상태 변경 감지
        if (wasSelected != isSelected)
        {
            wasSelected = isSelected;
            OnSelectionChanged(isSelected); // 여기서 원하는 함수 실행
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
        // 드래그 아이콘이 자동으로 마우스를 따라감 (Update에서 처리됨)
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

    private void OnSelectionChanged(bool isSelected)
    {
        if (isSelected)
        {
            Debug.Log($"[{index}] 슬롯이 선택됨: {slot.item?.itemName}");

            if (slot.item == null)
            {
                EventBus<WeaponChange>.Raise(new WeaponChange(ItemState.none));
                Debug.Log("선택된 아이템이 없음");
                return;
            }

            if (slot.item.weaponType==WeaponType.Sword)
            {
                EventBus<WeaponChange>.Raise(new WeaponChange(ItemState.Sword));
            }
            else if(slot.item.weaponType==WeaponType.Bow)
            {
                EventBus<WeaponChange>.Raise(new WeaponChange(ItemState.Bow));
            }
            else if(slot.item.weaponType==WeaponType.Axe)
            {
                EventBus<WeaponChange>.Raise(new WeaponChange(ItemState.Harberd));
            }
            else if(slot.item.potionType!=PotionType.NONE)
            {
                EventBus<WeaponChange>.Raise(new WeaponChange(ItemState.Position));
            }
            else if(slot.item.magicType==MagicType.Fire)
            {
                EventBus<WeaponChange>.Raise(new WeaponChange(ItemState.FireMagic));
            }
            else if (slot.item.magicType == MagicType.Ice)
            {
                EventBus<WeaponChange>.Raise(new WeaponChange(ItemState.IceMagic));
            }
            else if (slot.item.magicType == MagicType.Lightning)
            {
                EventBus<WeaponChange>.Raise(new WeaponChange(ItemState.ElectricMagic));
            }

        }
        else
        {
            Debug.Log($"[{index}] 슬롯 선택 해제");
        }
    }
}
