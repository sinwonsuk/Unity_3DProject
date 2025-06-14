using UnityEngine;

public class Hp : IEvent
{
    public int hp;

    public Hp(int _hp)
    {
        hp = _hp;
    }
}
