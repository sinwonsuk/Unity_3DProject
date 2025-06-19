using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;
using TMPro;

public class ShopSlot : MonoBehaviour
{
	public void Set(ItemData data, int itemPrice)
	{
		itemData = data;
		price = itemPrice;

		iconImage.sprite = itemData.itemIcon;

		slotButton.onClick.RemoveAllListeners();
		slotButton.onClick.AddListener(() => OnClicked?.Invoke());
	}

	public ItemData GetItemData() => itemData;
	public int GetPrice() => price;
	public Action OnClicked;

	[SerializeField] private Image iconImage;
	[SerializeField] private Button slotButton;

	private ItemData itemData;
	private int price;

	
}
