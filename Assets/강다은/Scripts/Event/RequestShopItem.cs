using System.Collections.Generic;

public class RequestShopItems : IEvent
{
	public string npcId;
	public System.Action<List<ShopItem>> OnResponse;

	public RequestShopItems(string npcId, System.Action<List<ShopItem>> onResponse)
	{
		this.npcId = npcId;
		this.OnResponse = onResponse;
	}
}
