using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/ShopManager")]
public class ShopManagerConfig : BaseScriptableObject
{
	[field: SerializeField] public List<NpcShopEntry> npcShops { get; private set; }
	public ShopManagerConfig()
	{
		type = typeof(ShopManagerConfig);
	}
}