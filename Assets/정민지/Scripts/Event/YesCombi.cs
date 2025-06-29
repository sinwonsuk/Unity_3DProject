using UnityEngine;

public class YesCombi :IEvent
{
    public bool canCombi;
    public YesCombi(bool canCombi)
    {
        this.canCombi = canCombi;
    }
}
