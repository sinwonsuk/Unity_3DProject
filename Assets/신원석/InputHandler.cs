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

        if (!data.buttons.IsSet(NetworkInputData.KEY_SPACE))
            return false;

        if (data.direction == Vector3.forward)
            rollDirection = ERollState.Forward;
        else if (data.direction == Vector3.back)
            rollDirection = ERollState.Backward;
        else if (data.direction == Vector3.left)
            rollDirection = ERollState.Left;
        else if (data.direction == Vector3.right)
            rollDirection = ERollState.Right;
        else
            return false;

        return true;
    }






    // ���� ��� �� ó��
    public bool TryGetMoveDirection(out Vector3 moveDir, out Quaternion planarRotation)
    {
        planarRotation = Quaternion.identity;
        moveDir = Vector3.zero;

        if (behaviour.GetInput(out NetworkInputData data))
        {
            Vector3 moveInput = new Vector3(data.moveAxis.x, 0, data.moveAxis.z).normalized;
            float yaw = data.CameraRotateY;
            planarRotation = Quaternion.Euler(0, yaw, 0);
            moveDir = planarRotation * moveInput;

            return moveInput.sqrMagnitude > 0.01f;
        }

        return false;
    }

    // ���� �Է� ó��
    public bool IsRightAttackPressed()
    {
        if (behaviour.GetInput(out NetworkInputData data))
        {      
            return data.buttons.IsSet(NetworkInputData.MOUSEBUTTON1);
        }

        return false;
    }

    public bool IsAttackPressed()
    {
        if (behaviour.GetInput(out NetworkInputData data))
        {
            return data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0);
        }
        return false;
    }

    // ��� ���� �Է� ó�� 
    public bool IsDashAttackPressed()
    {
        if (behaviour.GetInput(out NetworkInputData data))
        {
            return data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0) && data.buttons.IsSet(NetworkInputData.KEY_SPACE);
        }

        return false;
    }


    // ī�޶� ��ȯ 
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