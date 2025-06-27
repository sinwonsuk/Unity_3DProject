using UnityEngine;

public class isRunning : IEvent
{
    public bool imRunning;

    public isRunning(bool imRunning)
    {
        this.imRunning = imRunning;
    }
}
