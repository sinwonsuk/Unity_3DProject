using System.Collections.Generic;
using UnityEngine;

public class ItemBoxOpened : IEvent
{
	public List<ItemData> items;
	public GameObject itemBox; 

	public ItemBoxOpened(List<ItemData> items, GameObject box)
	{
		this.items = items;
		this.itemBox = box;
	}
}
