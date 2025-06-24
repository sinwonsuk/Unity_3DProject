using UnityEngine;

public struct WeaponChange :IEvent
{
    public ItemState state;

    public WeaponChange(ItemState state)
    {
        this.state = state;
    }

}
