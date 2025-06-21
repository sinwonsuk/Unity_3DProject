using Fusion;
using UnityEngine;
using static Unity.Collections.Unicode;

public class InputHandler
{
    private NetworkRunner runner;
    private NetworkBehaviour behaviour;
    private Transform transform;
    [Networked] private TickTimer delay { get; set; }
    public InputHandler(NetworkBehaviour behaviour, Transform transform)
    {
        this.behaviour = behaviour;
        this.runner = behaviour.Runner;
        this.transform = transform;
    }
    public bool RollInput(out ERollState rollDirection)
    {
        rollDirection = default;

        if (!behaviour.GetInput(out NetworkInputData data))
            return false;

        if (data.direction == Vector3.forward && data.buttons.IsSet(NetworkInputData.KEY_SPACE))
            rollDirection = ERollState.Forward;
        else if (data.direction == Vector3.back && data.buttons.IsSet(NetworkInputData.KEY_SPACE))
            rollDirection = ERollState.Backward;
        else if (data.direction == Vector3.left && data.buttons.IsSet(NetworkInputData.KEY_SPACE))
            rollDirection = ERollState.Left;
        else if (data.direction == Vector3.right && data.buttons.IsSet(NetworkInputData.KEY_SPACE))
            rollDirection = ERollState.Right;
        else
            return false;

        return true;
    }






    // 방향 계산 및 처리
    public void TryGetMoveDirection(out Vector3 moveDir, out Quaternion planarRotation)
    {
        planarRotation = Quaternion.identity;
        moveDir = Vector3.zero;

        if (behaviour.GetInput(out NetworkInputData data))
        {
            Vector3 moveInput = data.direction.normalized;
            float yaw = data.CameraRotateY;
            planarRotation = Quaternion.Euler(0, yaw, 0);
            moveDir = planarRotation * moveInput;
        }
    }

    // 공격 입력 처리
    public bool IsRightAttackPressed()
    {
        if (behaviour.GetInput(out NetworkInputData data))
        {      
            return data.buttons.IsSet(NetworkInputData.MOUSEBUTTON1);
        }

        return false;
    }

    public bool IsLButtonPress()
    {
        if (behaviour.GetInput(out NetworkInputData data))
        {
            return data.buttons.IsSet(NetworkInputData.KEY_L);
        }

        return false;
    }

    public bool IsCtrlButtonPress()
    {
        if (behaviour.GetInput(out NetworkInputData data))
        {
            return data.buttons.IsSet(NetworkInputData.KEY_L);
        }

        return false;
    }

    public bool IsMove()
    {
        if (behaviour.GetInput(out NetworkInputData data))
        {
            if(data.moveAxis.x != 0 || data.moveAxis.z != 0)
            {
                return true;
            }
        }
        return false;
    }

    public NetworkInputData GetNetworkInputData()
    {
        behaviour.GetInput(out NetworkInputData data);

        return data;
    }

    public bool IsAttackPressed()
    {
        if (behaviour.GetInput(out NetworkInputData data))
        {
            return data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0);
        }
        return false;
    }

    // 대시 공격 입력 처리 
    public bool IsDashAttackPressed()
    {
        if (behaviour.GetInput(out NetworkInputData data))
        {
            return data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0) && data.buttons.IsSet(NetworkInputData.KEY_SPACE);
        }

        return false;
    }


    // 카메라 전환 
    public bool ChangeCamera()
    {
        if (behaviour.GetInput(out NetworkInputData data))
        {
            if (data.buttons.IsSet(NetworkInputData.KEY_C))
            {
                return true;
            }
        }
        return false;
    }
}