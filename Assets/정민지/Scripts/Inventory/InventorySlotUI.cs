using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Image iconImage;
    public TMP_Text quantityText;
    public Image highlightImage;

    private int index;
    private BigInventoryUI owner;

    private Transform originalParent;
    private DraggedIcon draggedIcon;

    public void SetSlot(InventorySlot slot, bool isSelected = false)
    {
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

        highlightImage.enabled = isSelected;
    }

    public void Initialize(BigInventoryUI ownerUI, int slotIndex, DraggedIcon icon)
    {
        owner = ownerUI;
        index = slotIndex;
        draggedIcon = icon;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        owner.OnSlotClicked(index);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!iconImage.enabled) return;

        draggedIcon.gameObject.SetActive(true);
        draggedIcon.SetIcon(iconImage.sprite);
        draggedIcon.StartDrag(iconImage.sprite); // ← 여기서 호출!
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
            owner.SwapSlots(draggedSlotUI.index, this.index);
        }
    }
}
