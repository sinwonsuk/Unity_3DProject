using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public const int MOUSEBUTTON0 = 0;
    public const int KEY_C = 1;
    public const int KEY_SPACE = 2;
    public const int MOUSEBUTTON1 = 3;
    public const int KEY_L = 4;
    public const int KEY_CTRL = 5;
    public float CameraRotateY;

    public Vector2 LookRotationDelta;

    public Vector3 CameraForward;

    public Vector3 moveAxis;
    public NetworkButtons buttons;
    public Vector3 direction;
}