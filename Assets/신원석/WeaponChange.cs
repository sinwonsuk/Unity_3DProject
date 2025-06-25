using System;
using UnityEngine;

public struct WeaponChange :IEvent
{
    public ItemState state;

    public WeaponChange(ItemState state)
    {
        this.state = state;
    }

    public static explicit operator WeaponChange(int v)
    {
        throw new NotImplementedException();
    }
}
