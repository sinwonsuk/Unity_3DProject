using UnityEngine;

[CreateAssetMenu(menuName = "Config/Weapon")]
public class WeaponInfoConfig : ScriptableObject
{
    [SerializeField] private int attack;
    [SerializeField] private int stamina;
    [SerializeField] Vector3 scaleOne;
    [SerializeField] Vector3 scaleTwo;
    [SerializeField] Vector3 scaleThree;

    public int Attack
    {
        get => attack;
        set => attack = value;
    }

    public Vector3 ScaleOne
    {
        get => scaleOne;
        set => scaleOne = value;
    }
    public Vector3 ScaleTwo
    {
        get => scaleTwo;
        set => scaleTwo = value;
    }
    public Vector3 ScaleThree
    {
        get => scaleThree;
        set => scaleThree = value;
    }
    public int Stamina
    {
        get => stamina;
        set => stamina = value;
    }
}
