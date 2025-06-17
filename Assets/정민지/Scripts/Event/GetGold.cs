using UnityEngine;

public class GetGold : IEvent
{
    public int getGold;

    public GetGold(int getGold)
    {
        this.getGold = getGold;
    }
}
