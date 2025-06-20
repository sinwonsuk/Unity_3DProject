using UnityEngine;

public class ItemBoxUIClose : IEvent
{
	public GameObject targetBox;
	public ItemBoxUIClose(GameObject box) => targetBox = box;
}
