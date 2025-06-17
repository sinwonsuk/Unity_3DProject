using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.UI.GridLayoutGroup;

public class CombinationSlotUI : MonoBehaviour,IPointerClickHandler
{
    public Image cImage;
    public ItemData cItem;
    [SerializeField] private Inventory inventory;
    [SerializeField] private Sprite emptySprite; // 빈 칸에 사용할 스프라이트

    public void CSlotClear()
    {
        cItem = null;
        cImage.sprite = emptySprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            TryReturnItemToInventory();
        }
    }

    private void TryReturnItemToInventory()
    {
        if (cItem == null || inventory == null)
            return;

        inventory.AddItem(cItem); // 인벤토리에 추가
        Debug.Log($"[슬롯] {cItem.itemName} 인벤토리에 반환됨");

        CSlotClear();
    }

}
