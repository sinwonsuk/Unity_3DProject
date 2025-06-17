using UnityEngine;

public class Hp : IEvent
{
    public int currentHp;

    public Hp(int _hp)
    {
        currentHp = _hp;
    }
}
