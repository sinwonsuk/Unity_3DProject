using System.Collections.Generic;
using UnityEngine;

public class ShopManager : baseManager, IGameManager
{
	private ShopManagerConfig config;
	private Dictionary<int, ShopItem> shopItemDict = new();

	public ShopManager(ShopManagerConfig config)
	{
		this.config = config;
	}

	public ShopManager(BaseScriptableObject baseScriptableObject)
	{
		type = typeof(ShopManager);
		config = (ShopManagerConfig)baseScriptableObject;
	}

	public override void Init()
	{
		foreach (var item in config.shopItems)
		{
			if (!shopItemDict.ContainsKey(item.itemId))
				shopItemDict.Add(item.itemId, item);
		}

		Debug.Log($"ShopManager 초기화 완료: {shopItemDict.Count}개 상품 등록됨");
	}
	public List<ShopItem> GetShopItems()
	{
		return config.shopItems;
	}

	public bool TryPurchase(int itemId, int playerGold, out int newGold)
	{
		var item = config.shopItems.Find(i => i.itemId == itemId);
		if (item == null || playerGold < item.price)
		{
			newGold = playerGold;
			return false;
		}

		newGold = playerGold - item.price;
		return true;
	}
}
