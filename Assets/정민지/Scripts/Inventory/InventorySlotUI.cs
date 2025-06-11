using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
{
    public Image iconImage;
    public TMP_Text quantityText;
    public Image highlightImage;

    private int index; // 슬롯 인덱스 저장
    private BigInventoryUI owner; // 부모 UI 참조

    public void SetSlot(InventorySlot slot, bool isSelected = false) //아이템 세팅
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
           // quantityText.text = slot.item.isStackable ? slot.quantity.ToString() : "";
        }

        highlightImage.enabled = isSelected;
    }

    public void Initialize(BigInventoryUI ownerUI, int slotIndex)
    {
        owner = ownerUI;
        index = slotIndex;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        owner.OnSlotClicked(index);

    }

}
