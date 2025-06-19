using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class ShopManager : baseManager, IGameManager
{
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
		foreach (var item in config.npcShops)
		{
			if (!npcShopTable.ContainsKey(item.npcId))
				npcShopTable[item.npcId] = item.shopItems;
		}

		EventBus<RequestShopItems>.OnEvent += HandleShopItemRequest;

		Debug.Log($"[ShopManager] 초기화 완료: {npcShopTable.Count}개 NPC 상점 로드됨");
	}

	private void HandleShopItemRequest(RequestShopItems evt)
	{
		var list = GetShopItemsForNpc(evt.npcId);
		evt.OnResponse?.Invoke(list);
	}

	public List<ShopItem> GetShopItemsForNpc(string npcId)
	{
		return npcShopTable.TryGetValue(npcId, out var list) ? list : new List<ShopItem>();
	}

	private ShopManagerConfig config;
	private Dictionary<string, List<ShopItem>> npcShopTable = new();
}
