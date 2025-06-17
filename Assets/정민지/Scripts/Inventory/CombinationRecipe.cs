using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Combination Recipe")]
public class CombinationRecipe : ScriptableObject
{
    public List<ItemData> ingredients = new List<ItemData>(3);  // ��� 3��
    public ItemData result;                                     // ��� ������
}
