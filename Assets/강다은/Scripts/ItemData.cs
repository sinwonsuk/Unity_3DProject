using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Item/ItemData")]
public class ItemData : ScriptableObject
{
	public int itemId;
	public string itemName;
	public Sprite itemIcon;

	public ItemType itemType;
	public int itemGrade; // 1~3성

	public WeaponType weaponType;
	public MagicType magicType;
	public PotionType potionType;

	public GameObject itemPrefab;

	public int price;     // 상점 판매가
	public string description;

    public bool isStackable; //동일한 아이템 유무
}
