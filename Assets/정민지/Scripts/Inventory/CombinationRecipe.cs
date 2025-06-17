using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Combination Recipe")]
public class CombinationRecipe : ScriptableObject
{
    public List<ItemData> ingredients = new List<ItemData>(3);  // 재료 3개
    public ItemData result;                                     // 결과 아이템
}
