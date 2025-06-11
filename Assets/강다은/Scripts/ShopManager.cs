using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class ShopManager : baseManager, IGameManager
{
	private ShopManagerConfig config;
	private Dictionary<string, List<ShopItem>> npcShopTable = new();

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

		Debug.Log($"ShopManager �ʱ�ȭ �Ϸ�: {npcShopTable.Count}�� ��ǰ ��ϵ�");
	}

	public List<ShopItem> GetShopItems(string npcId)
	{
		return npcShopTable.TryGetValue(npcId, out var items) ? items : new List<ShopItem>();
	}
}
