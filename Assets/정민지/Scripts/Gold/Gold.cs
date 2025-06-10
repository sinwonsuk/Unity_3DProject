using TMPro;
using UnityEngine;

public class Gold : IEvent
{
    public int currentGold;

    public Gold(int _gold)
    {
        currentGold = _gold;
    }
}
