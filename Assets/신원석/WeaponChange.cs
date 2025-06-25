using Fusion;
using System;
using UnityEngine;

public struct WeaponChange :IEvent
{
    public ItemState state;
    public RpcInfo info;
    public PlayerRef inf2;
    public WeaponChange(ItemState state, RpcInfo info, PlayerRef inf2)
    {
        this.state = state;
        this.info = info;
        this.inf2 = inf2;
    }

    public static explicit operator WeaponChange(int v)
    {
        throw new NotImplementedException();
    }
}
