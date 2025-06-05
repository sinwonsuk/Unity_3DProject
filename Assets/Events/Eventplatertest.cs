using UnityEngine;

public struct Eventplatertest : IEvent
{
    public playertest playertest;
    public int ads;

    public Eventplatertest(int hp)
    {
        ads = 40;

        playertest = null;
    }

}
