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

		// 마우스 휠
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
			Debug.Log($"[Shop] {selectedItemData.itemName}, {currentAmount}개, {selectedItemData.price * currentAmount} G 구매 요청 발생");
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

	[SerializeField] private Transform slotRoot;               // 슬롯 부모
	[SerializeField] private ShopSlot slotPrefab;              // 슬롯 프리팹
	[SerializeField] private GameObject detailPanel;           // 상세 정보 패널
	[SerializeField] private Image detailItemIcon;             // 상세 아이콘
	[SerializeField] private TMP_Text detailItemName;              // 상세 이름
	[SerializeField] private TMP_Text detailItemPrice;             // 상세 가격
	[SerializeField] private TMP_Text detailItemDescription;      // 상세 설명 
	[SerializeField] private Button buyButton;                 // 구매 버튼
	[SerializeField] private Button increaseButton;            // 수량 증가 버튼
	[SerializeField] private Button decreaseButton;            // 수량 감소 버튼
	[SerializeField] private TMP_Text amountText;              // 현재 수량 표시


	private int currentAmount = 1; // 현재 선택된 아이템 수량
	private int maxAmount = 99; // 최대 수량
	private int minAmount = 1; // 최소 수량
	private int currentItemPrice; // 현재 아이템 가격

	private float holdDelay = 0.4f;
	private float holdSpeed = 0.1f;
	private float holdTimer = 0f;
	private bool isHoldingLeft = false;
	private bool isHoldingRight = false;

	private List<ShopSlot> slotInstances = new();
	private ItemData selectedItemData;


}
