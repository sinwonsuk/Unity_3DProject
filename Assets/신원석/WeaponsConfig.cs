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



using Fusion;
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
        public NetworkPrefabRef weaponPrefab;
        public Transform transform;
    }

    [SerializeField]
    private List<WeaponEntry> weaponEntries;

    private Dictionary<ItemState, Transform> dicWeaponsTransform;
    private Dictionary<ItemState, NetworkPrefabRef> dicWeapons;

    private void OnEnable()
    {
        dicWeapons = new Dictionary<ItemState, NetworkPrefabRef>();
        dicWeaponsTransform = new Dictionary<ItemState, Transform>();

        foreach (var entry in weaponEntries)
        {
            if (!dicWeapons.ContainsKey(entry.itemState))
                dicWeapons.Add(entry.itemState, entry.weaponPrefab);

            if (!dicWeaponsTransform.ContainsKey(entry.itemState))
                dicWeaponsTransform.Add(entry.itemState, entry.transform);


        }
    }

    public NetworkPrefabRef GetWeapon(ItemState state)
    {
        if (dicWeapons.TryGetValue(state, out var weapon))
            return weapon;

        Debug.LogWarning($"Weapon for {state} not found!");
        return default;
    }
    public Transform GetTransform(ItemState state)
    {
        if (dicWeaponsTransform.TryGetValue(state, out var weapon))
            return weapon;

        Debug.LogWarning($"Weapon for {state} not found!");
        return default;
    }
    public List<NetworkPrefabRef> GetWeaponGameObjects()
    {
        List<NetworkPrefabRef> list = new();
        foreach (var entry in weaponEntries)
            list.Add(entry.weaponPrefab);
        return list;
    }
}
