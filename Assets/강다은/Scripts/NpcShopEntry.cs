using System.Collections.Generic;

[System.Serializable]
public class NpcShopEntry
{
	public string npcId;                    // 예: "npc_blacksmith"
	public List<ShopItem> shopItems;        // 이 NPC가 파는 아이템 목록
}
