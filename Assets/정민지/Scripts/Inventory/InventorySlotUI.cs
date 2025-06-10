using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    public Image iconImage;
    public TMP_Text quantityText;
    public Image highlightImage;

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
            quantityText.text = slot.item.isStackable ? slot.quantity.ToString() : "";
        }

        highlightImage.enabled = isSelected;
    }
}
