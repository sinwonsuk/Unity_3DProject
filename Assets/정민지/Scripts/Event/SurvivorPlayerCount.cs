using UnityEngine;

public class SurvivorPlayerCount : IEvent
{
    public int pCount;

    public SurvivorPlayerCount(int count)
    {
        pCount = count;
    }

}
