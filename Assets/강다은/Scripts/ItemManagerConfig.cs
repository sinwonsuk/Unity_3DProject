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
}

[System.Serializable]
public class InitialInventoryEntry
{
	public int itemId;
	public int amount;
}
