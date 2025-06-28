using Fusion;
using System;
using UnityEngine;

public struct WeaponChange :IEvent
{
    public ItemState state;
    public RpcInfo info;
    public PlayerRef inf2;
    public int num;
    public WeaponChange(ItemState state, RpcInfo info, PlayerRef inf2, int num)
    {
        this.state = state;
        this.info = info;
        this.inf2 = inf2;

        this.num = num;

    }

    public static explicit operator WeaponChange(int v)
    {
        throw new NotImplementedException();
    }
}
