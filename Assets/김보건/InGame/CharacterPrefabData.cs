using System.Collections.Generic;
using UnityEngine;
using Fusion; 

[CreateAssetMenu(menuName = "Game/CharacterPrefabData")]
public class CharacterPrefabData : ScriptableObject
{
    public List<CharacterPrefabEntry> characterPrefabs;
}

[System.Serializable]
public class CharacterPrefabEntry
{
    public string characterName;
    public NetworkPrefabRef prefab;
}