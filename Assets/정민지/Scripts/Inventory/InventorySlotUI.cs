using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
{
    public Image iconImage;
    public TMP_Text quantityText;
    public Image highlightImage;

    private int index; // ���� �ε��� ����
    private BigInventoryUI owner; // �θ� UI ����

    public void SetSlot(InventorySlot slot, bool isSelected = false) //������ ����
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
