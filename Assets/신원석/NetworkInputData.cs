using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public const byte MOUSEBUTTON0 = 1;
    public const byte KEY_C = 1 << 1;
    public const byte KEY_SPACE = 1 << 2;
    public const byte MOUSEBUTTON1 = 1 << 3;
    public float CameraRotateY;
    public Vector3 test;

    public Vector3 moveAxis;
    public NetworkButtons buttons;
    public Vector3 direction;
}