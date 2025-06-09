using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : baseManager, IGameManager
{
	ItemManagerConfig config;
	private Dictionary<int, ItemData> itemDict = new();
	public ItemManager(ItemManagerConfig itemManagerConfig)
	{
		config = itemManagerConfig;
	}
	public ItemManager(BaseScriptableObject baseScriptableObject)
	{
		type = typeof(ItemManager);
		config = (ItemManagerConfig)baseScriptableObject;
	}
	public override void Init()
	{
		foreach (var item in config.itemList)
		{
			if(!itemDict.ContainsKey(item.itemId))
			{
				itemDict.Add(item.itemId, item);
			}

			Debug.Log($"ItemManager 초기화 완료: {itemDict.Count}개 아이템 로드됨");

		}
	}
	public ItemData GetItem(int itemId)
	{
		return itemDict.TryGetValue(itemId, out var data) ? data : null;
	}

	public List<ItemData> GetAllItems()
	{
		return new List<ItemData>(itemDict.Values);
	}
}