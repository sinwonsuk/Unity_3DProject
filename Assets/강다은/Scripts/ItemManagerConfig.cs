using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Config/ItemManager")]
public class ItemManagerConfig : BaseScriptableObject
{
	public ItemManagerConfig()
	{
		type = typeof(ItemManagerConfig);
	}

	[field: SerializeField]
	public List<ItemData> itemList { get; private set; }

	[field: SerializeField]
	public List<InitialInventoryEntry> InitialInventory { get; private set; }

	public bool CanUpgrade(ItemData currentItem, List<ItemData> playerInventory)
	{
		if (currentItem.itemGrade >= 3)		// 현재 등급이 최대면 업그레이드 불가
			return false;

		// 동일한 아이템 3개 이상 있는지 확인
		int count = 0;
		foreach (var item in playerInventory)
		{
			if (item.itemType == currentItem.itemType &&
				item.itemGrade == currentItem.itemGrade &&
				item.weaponType == currentItem.weaponType &&
				item.magicType == currentItem.magicType)
			{
				count++;
			}
		}
		return count >= 3;
	}

}

[System.Serializable]
public class InitialInventoryEntry
{
	public int itemId;
	public int amount;
}


