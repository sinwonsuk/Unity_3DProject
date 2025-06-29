using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class ShopSlot : MonoBehaviour, IPointerClickHandler
{
    public Action OnRightClick;

    // ���� Set �޼���� �״��
    public void Set(ItemData data, int itemPrice)
    {
        itemData = data;
        price = itemPrice;
        iconImage.sprite = itemData.itemIcon;
        // �� �̻� slotButton.onClick ���� �ƹ��͵� �������� �ʽ��ϴ�.
    }

    // ��Ŭ���� üũ
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
