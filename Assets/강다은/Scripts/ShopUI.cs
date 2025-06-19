using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ShopUI : MonoBehaviour
{

	private void Update()
	{
		if (!detailPanel.activeSelf) return;

		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			SetAmount(currentAmount >= maxAmount ? 1 : currentAmount + 1);
			isHoldingRight = true;
			holdTimer = holdDelay;
		}
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			SetAmount(currentAmount <= 1 ? maxAmount : currentAmount - 1);
			isHoldingLeft = true;
			holdTimer = holdDelay;
		}

		if (Input.GetKeyUp(KeyCode.RightArrow))
			isHoldingRight = false;
		if (Input.GetKeyUp(KeyCode.LeftArrow))
			isHoldingLeft = false;

		if (isHoldingRight || isHoldingLeft)
		{
			holdTimer -= Time.unscaledDeltaTime;
			if (holdTimer <= 0f)
			{
				holdTimer = holdSpeed;

				if (isHoldingRight)
					SetAmount(currentAmount >= maxAmount ? 1 : currentAmount + 1);
				else if (isHoldingLeft)
					SetAmount(currentAmount <= 1 ? maxAmount : currentAmount - 1);
			}
		}

		// ���콺 ��
		float scroll = Input.GetAxis("Mouse ScrollWheel");
		if (scroll > 0f)
			SetAmount(currentAmount >= maxAmount ? 1 : currentAmount + 1);
		else if (scroll < 0f)
			SetAmount(currentAmount <= 1 ? maxAmount : currentAmount - 1);
	}


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
		currentItemPrice = shopItem.price;
		detailItemDescription.text = selectedItemData.description;
		detailPanel.SetActive(true);

		detailItemIcon.sprite = selectedItemData.itemIcon;
		detailItemName.text = selectedItemData.itemName;
		detailItemPrice.text = $"{shopItem.price} G";

		SetAmount(minAmount);

		buyButton.onClick.RemoveAllListeners();
		buyButton.onClick.AddListener(() =>
		{

			EventBus<BuyItemRequested>.Raise(new BuyItemRequested(selectedItemData, currentAmount, selectedItemData.price));
			Debug.Log($"[Shop] {selectedItemData.itemName}, {currentAmount}��, {selectedItemData.price * currentAmount} G ���� ��û �߻�");
		});
		increaseButton.onClick.RemoveAllListeners();
		increaseButton.onClick.AddListener(() => SetAmount(currentAmount + 1));

		decreaseButton.onClick.RemoveAllListeners();
		decreaseButton.onClick.AddListener(() => SetAmount(currentAmount - 1));
	}

	private void SetAmount(int amount)
	{
		currentAmount = Mathf.Clamp(amount, 1, maxAmount);
		amountText.text = currentAmount.ToString();
		detailItemPrice.text = $"{currentItemPrice * currentAmount} G";
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

	[SerializeField] private Transform slotRoot;               // ���� �θ�
	[SerializeField] private ShopSlot slotPrefab;              // ���� ������
	[SerializeField] private GameObject detailPanel;           // �� ���� �г�
	[SerializeField] private Image detailItemIcon;             // �� ������
	[SerializeField] private TMP_Text detailItemName;              // �� �̸�
	[SerializeField] private TMP_Text detailItemPrice;             // �� ����
	[SerializeField] private TMP_Text detailItemDescription;      // �� ���� 
	[SerializeField] private Button buyButton;                 // ���� ��ư
	[SerializeField] private Button increaseButton;            // ���� ���� ��ư
	[SerializeField] private Button decreaseButton;            // ���� ���� ��ư
	[SerializeField] private TMP_Text amountText;              // ���� ���� ǥ��


	private int currentAmount = 1; // ���� ���õ� ������ ����
	private int maxAmount = 99; // �ִ� ����
	private int minAmount = 1; // �ּ� ����
	private int currentItemPrice; // ���� ������ ����

	private float holdDelay = 0.4f;
	private float holdSpeed = 0.1f;
	private float holdTimer = 0f;
	private bool isHoldingLeft = false;
	private bool isHoldingRight = false;

	private List<ShopSlot> slotInstances = new();
	private ItemData selectedItemData;


}
