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

	public EffectType effectType;    // 효과 종류
	public float effectValue;		 // 효과 수치
}
