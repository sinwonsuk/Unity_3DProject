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

		ShowModel(selectedItemData.ItemModelPrefab, selectedItemData);
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

	[SerializeField] private Transform slotRoot;               // ���� �θ�
	[SerializeField] private ShopSlot slotPrefab;              // ���� ������
	[SerializeField] private GameObject detailPanel;           // �� ���� �г�
	//[SerializeField] private Image detailItemIcon;             // �� ������
	[SerializeField] private TMP_Text detailItemName;              // �� �̸�
	[SerializeField] private TMP_Text detailItemPrice;             // �� ����
	[SerializeField] private TMP_Text detailItemDescription;      // �� ���� 
	[SerializeField] private Button buyButton;                 // ���� ��ư
	[SerializeField] private Button increaseButton;            // ���� ���� ��ư
	[SerializeField] private Button decreaseButton;            // ���� ���� ��ư
	[SerializeField] private TMP_Text amountText;              // ���� ���� ǥ��
	[SerializeField] private Transform modelDisplayRoot;
	[SerializeField] private RawImage modelPreviewImage;
	[SerializeField] private RenderTexture modelRenderTexture;
	[SerializeField] private Camera modelPreviewCamera;

	private GameObject currentModel;


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
