using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/ShopManager")]
public class ShopManagerConfig : BaseScriptableObject
{
	public ShopManagerConfig()
	{
		type = typeof(ShopManagerConfig);
	}

	[field: SerializeField]
	public List<ShopItem> shopItems { get; private set; }
}
