public class RequestItemToInventory : IEvent
{
	public ItemData itemData;
	public int amount;

	public RequestItemToInventory(ItemData data, int count)
	{
		itemData = data;
		amount = count;
	}
}
