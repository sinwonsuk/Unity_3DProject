using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public const byte MOUSEBUTTON0 = 1;
    public const byte KEY_C = 1 << 1;
    public Vector3 moveAxis;
    public NetworkButtons buttons;
    public Vector3 direction;
}