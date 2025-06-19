
using System;
using UnityEngine;

public struct Eventplatertest : IEvent
{
    public playertest playertest;
    public GameObject objdasdadect;
    public int ads;

    public Eventplatertest(GameObject adwd)
    {
        objdasdadect = adwd;


        ads = 40;

        playertest = null;
    }
    //public Eventplatertest()
    //{
    //    playertest = null;
    //    ads = 0;
    //}
}
