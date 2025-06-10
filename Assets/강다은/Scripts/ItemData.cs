using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Item/ItemData")]
public class ItemData : ScriptableObject
{
	public int itemId;
	public string itemName;
	public Sprite itemIcon;

	public ItemType itemType;
	public int itemGrade; // 1~3��

	public WeaponType weaponType;
	public MagicType magicType;
	public PotionType potionType;

	public GameObject itemPrefab;

	public int price;     // ���� �ǸŰ�
	public string description;

    public bool isStackable; //������ ������ ����
}
