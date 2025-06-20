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

		ShowModel(selectedItemData.ItemModelPrefab, selectedItemData);
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

	private void ShowModel(GameObject prefab, ItemData itemData)
	{
		if (currentModel != null)
			Destroy(currentModel);

		if (prefab == null)
		{
			modelPreviewImage.gameObject.SetActive(false);
			return;
		}
		modelDisplayRoot.transform.localPosition = Vector3.zero;
		modelDisplayRoot.transform.localRotation = Quaternion.identity;

		currentModel = Instantiate(prefab, modelDisplayRoot);
		
		currentModel.transform.localPosition = Vector3.zero;

		switch (itemData.weaponType)
		{
			case WeaponType.Bow:
				modelDisplayRoot.transform.localPosition = new Vector3(0f, 2f, 0f);
				currentModel.transform.localRotation = Quaternion.Euler(30f, 45f, 0f);
				currentModel.transform.localScale = Vector3.one * 3f;
				break;
			case WeaponType.Spear:
				currentModel.transform.localRotation = Quaternion.Euler(30f, 45f, 0f);
				currentModel.transform.localScale = Vector3.one * 2f;
				break;
			default:
				currentModel.transform.localRotation = Quaternion.Euler(30f, 45f, 0f);
				currentModel.transform.localScale = Vector3.one * 4f;
				break;

		}
		if(itemData.itemType == ItemType.Magic)
		{
			modelDisplayRoot.transform.localPosition = new Vector3(0f, 2f, 0f);
			currentModel.transform.localRotation = Quaternion.Euler(-82f, 45f, -15f);
			currentModel.transform.localScale = Vector3.one * 5f;
		}
		else if (itemData.itemType == ItemType.Potion)
		{
			currentModel.transform.localRotation = Quaternion.Euler(0f, 15f, 0f);
			currentModel.transform.localScale = Vector3.one * 7f;
		}

		SetLayerRecursively(currentModel, LayerMask.NameToLayer("ModlePreview"));
		modelPreviewImage.gameObject.SetActive(true);
	}
	private void SetLayerRecursively(GameObject obj, int layer)
	{
		obj.layer = layer;
		foreach (Transform child in obj.transform)
		{
			SetLayerRecursively(child.gameObject, layer);
		}
	}
	//private void AutoFitModel(GameObject model, float targetSize = 7.0f)
	//{
	//	var renderers = model.GetComponentsInChildren<Renderer>();
	//	if (renderers.Length == 0) return;
	//
	//	Bounds combinedBounds = renderers[0].bounds;
	//	foreach (var r in renderers)
	//	{
	//		combinedBounds.Encapsulate(r.bounds);
	//	}
	//
	//	float maxDimension = Mathf.Max(combinedBounds.size.x, combinedBounds.size.y, combinedBounds.size.z);
	//	if (maxDimension <= 0.0001f) maxDimension = 1f; // fallback
	//
	//	float scaleFactor = targetSize / maxDimension;
	//	model.transform.localScale = Vector3.one * scaleFactor;
	//}

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
	//[SerializeField] private Image detailItemIcon;             // 상세 아이콘
	[SerializeField] private TMP_Text detailItemName;              // 상세 이름
	[SerializeField] private TMP_Text detailItemPrice;             // 상세 가격
	[SerializeField] private TMP_Text detailItemDescription;      // 상세 설명 
	[SerializeField] private Button buyButton;                 // 구매 버튼
	[SerializeField] private Button increaseButton;            // 수량 증가 버튼
	[SerializeField] private Button decreaseButton;            // 수량 감소 버튼
	[SerializeField] private TMP_Text amountText;              // 현재 수량 표시
	[SerializeField] private Transform modelDisplayRoot;
	[SerializeField] private RawImage modelPreviewImage;
	[SerializeField] private RenderTexture modelRenderTexture;
	[SerializeField] private Camera modelPreviewCamera;

	private GameObject currentModel;


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
