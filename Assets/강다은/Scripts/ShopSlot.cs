using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class ShopSlot : MonoBehaviour, IPointerClickHandler
{
    public Action OnRightClick;

    // 기존 Set 메서드는 그대로
    public void Set(ItemData data, int itemPrice)
    {
        itemData = data;
        price = itemPrice;
        iconImage.sprite = itemData.itemIcon;
        // 더 이상 slotButton.onClick 에는 아무것도 연결하지 않습니다.
    }

    // 우클릭만 체크
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick?.Invoke();
        }
    }

    [SerializeField] private UnityEngine.UI.Image iconImage;
    private ItemData itemData;
    private int price;
}
