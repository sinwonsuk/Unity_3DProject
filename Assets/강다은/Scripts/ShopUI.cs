using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ShopUI : MonoBehaviour
{
	[SerializeField] private Transform slotRoot;               // 슬롯 부모
	[SerializeField] private ShopSlot slotPrefab;              // 슬롯 프리팹
	[SerializeField] private GameObject detailPanel;           // 상세 정보 패널
	[SerializeField] private Image detailItemIcon;             // 상세 아이콘
	[SerializeField] private TMP_Text detailItemName;              // 상세 이름
	[SerializeField] private TMP_Text detailItemPrice;             // 상세 가격
	[SerializeField] private Button buyButton;                 // 구매 버튼

	private List<ShopSlot> slotInstances = new();
	private ItemData selectedItemData;
	public void OpenShop(string npcId)
	{
		EventBus<RequestShopItems>.Raise(new RequestShopItems(npcId, shopItems =>
		{
			Initialize(shopItems);
		}));
	}

	public void Initialize(List<ShopItem> shopItems)
	{
		// 기존 슬롯 정리
		foreach (var slot in slotInstances)
			Destroy(slot.gameObject);
		slotInstances.Clear();

		// 새 슬롯 생성
		foreach (var shopItem in shopItems)
		{
			var slot = Instantiate(slotPrefab, slotRoot);
			slot.Set(shopItem.itemData, shopItem.price);
			slotInstances.Add(slot);

			slot.OnClicked = () =>
			{
				SelectItem(shopItem);
			};
		}

		detailPanel.SetActive(false); // 초기에는 숨김
	}

	private void SelectItem(ShopItem shopItem)
	{
		selectedItemData = shopItem.itemData;
		detailPanel.SetActive(true);

		detailItemIcon.sprite = selectedItemData.itemIcon;
		detailItemName.text = selectedItemData.itemName;
		detailItemPrice.text = $"{shopItem.price} G";

		buyButton.onClick.RemoveAllListeners();
		buyButton.onClick.AddListener(() =>
		{
			int amount = 1; // 수량 

			EventBus<BuyItemRequested>.Raise(new BuyItemRequested(selectedItemData, amount, selectedItemData.price));
			Debug.Log($"[Shop] {selectedItemData.itemName} 구매 요청 발생");
		});
	}

	public void Show()
	{
		gameObject.SetActive(true);
		detailPanel.SetActive(false);
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}

}
