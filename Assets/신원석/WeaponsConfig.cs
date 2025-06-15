//using NUnit.Framework;
//using System;
//using System.Collections.Generic;
//using UnityEngine;

//[CreateAssetMenu(menuName = "Config/Weapons")]

//public class WeaponsConfig
//{



//    public WeaponsConfig()
//    {
//        for (int i = 0; i < Weapons.Count; i++)
//        {
//            dicWeapons.Add((ItemState)i, Weapons[0]);
//        }
//    }

//    public List<GameObject> GetWeaponGameObjects()
//    {
//        return Weapons;
//    }

//    [field: SerializeField]
//    private List<GameObject> Weapons { get; set; }

//    Dictionary<ItemState, GameObject> dicWeapons = new Dictionary<ItemState, GameObject>();

//}



using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Weapons")]
public class WeaponsConfig : ScriptableObject
{
    [Serializable]
    public class WeaponEntry
    {
        public ItemState itemState;
        public GameObject weaponPrefab;
    }

    [SerializeField]
    private List<WeaponEntry> weaponEntries;

    private Dictionary<ItemState, GameObject> dicWeapons;

    private void OnEnable()
    {
        dicWeapons = new Dictionary<ItemState, GameObject>();

        foreach (var entry in weaponEntries)
        {
            if (!dicWeapons.ContainsKey(entry.itemState))
                dicWeapons.Add(entry.itemState, entry.weaponPrefab);
        }
    }

    public GameObject GetWeapon(ItemState state)
    {
        if (dicWeapons.TryGetValue(state, out var weapon))
            return weapon;

        Debug.LogWarning($"Weapon for {state} not found!");
        return null;
    }

    public List<GameObject> GetWeaponGameObjects()
    {
        List<GameObject> list = new();
        foreach (var entry in weaponEntries)
            list.Add(entry.weaponPrefab);
        return list;
    }
}
