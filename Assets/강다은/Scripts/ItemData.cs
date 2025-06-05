using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Item/Item")]
public class ItemData : ScriptableObject
{
	public int itemId;
	public string itemName;
	public Sprite icon;
	public ItemData nextUpgradeItem;
}
