using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;

public class ItemBoxUI : MonoBehaviour
{
	private void OnEnable()
	{
		Debug.Log("[ItemBoxUI] OnEnable");
		EventBus<ItemBoxOpened>.OnEvent += ShowItems;
		EventBus<ItemBoxUIClose>.OnEvent += CloseUI;
	}

	private void OnDisable()
	{
		EventBus<ItemBoxOpened>.OnEvent -= ShowItems;
		EventBus<ItemBoxUIClose>.OnEvent += CloseUI;
	}

	private void CloseUI(ItemBoxUIClose evt)
	{
		itemBoxCanvas.SetActive(false);
		currentBox = null;
	}

	private void ShowItems(ItemBoxOpened evt)
	{
		Debug.Log("[ItemBoxUI] ShowItems 호출됨");
		
		currentBox = evt.itemBox;
		selectedItems = evt.items;
		itemBoxCanvas.SetActive(true);

		foreach (Transform child in slotParent)
			Destroy(child.gameObject);

		foreach (var item in selectedItems)
		{
			var slot = Instantiate(slotPrefab, slotParent);
			slot.GetComponent<ShopSlot>().Set(item, item.price);
			GameObject slotToRemove = slot.gameObject;

			slot.GetComponent<ShopSlot>().OnClicked = () =>
			{
				OnSlotClicked(item, slotToRemove);
			};
		}
	}

	private void OnSlotClicked(ItemData item, GameObject slotToRemove)
	{
		Debug.Log($"[ItemSlot] {item.itemName} 클릭됨, 인벤토리에 추가 요청");
		EventBus<RequestItemToInventory>.Raise(new RequestItemToInventory(item, 1));

		Destroy(slotToRemove);
	}

	[SerializeField] private GameObject itemBoxCanvas;
	[SerializeField] private Transform slotParent;
	[SerializeField] private GameObject slotPrefab;

	private GameObject currentBox;
	private List<ItemData> selectedItems = new List<ItemData>();
}
