using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Zone Config")]
public class ZoneConfig : ScriptableObject
{
    public List<ZonePhase> phases;
}

[System.Serializable]
public class ZonePhase
{
    public Vector3 center;
    public float radius;
    public float waitDuration;
    public float shrinkDuration;
    public float damagePerSecond;
}
