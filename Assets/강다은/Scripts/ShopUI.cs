using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ShopUI : MonoBehaviour
{
	[SerializeField] private Transform slotRoot;               // ���� �θ�
	[SerializeField] private ShopSlot slotPrefab;              // ���� ������
	[SerializeField] private GameObject detailPanel;           // �� ���� �г�
	[SerializeField] private Image detailItemIcon;             // �� ������
	[SerializeField] private TMP_Text detailItemName;              // �� �̸�
	[SerializeField] private TMP_Text detailItemPrice;             // �� ����
	[SerializeField] private Button buyButton;                 // ���� ��ư

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
		// ���� ���� ����
		foreach (var slot in slotInstances)
			Destroy(slot.gameObject);
		slotInstances.Clear();

		// �� ���� ����
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

		detailPanel.SetActive(false); // �ʱ⿡�� ����
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
			int amount = 1; // ���� 

			EventBus<BuyItemRequested>.Raise(new BuyItemRequested(selectedItemData, amount, selectedItemData.price));
			Debug.Log($"[Shop] {selectedItemData.itemName} ���� ��û �߻�");
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
