using UnityEngine;

public class BuyItemRequested : IEvent
{
	public ItemData itemData;
	public int amount;
	public int price;

	public BuyItemRequested(ItemData itemData, int amount, int price)
	{
		this.itemData = itemData;
		this.amount = amount;
		this.price = itemData.price;
	}
}
