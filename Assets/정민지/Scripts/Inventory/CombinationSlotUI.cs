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
    [SerializeField] private Sprite emptySprite; // �� ĭ�� ����� ��������Ʈ

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

        inventory.AddItem(cItem); // �κ��丮�� �߰�
        Debug.Log($"[����] {cItem.itemName} �κ��丮�� ��ȯ��");

        CSlotClear();
    }

}
