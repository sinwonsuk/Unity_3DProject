using Fusion;
using UnityEngine;

public class InputHandler
{
    private NetworkRunner runner;
    private NetworkBehaviour behaviour;

    public InputHandler(NetworkBehaviour behaviour)
    {
        this.behaviour = behaviour;
        this.runner = behaviour.Runner;
    }

    // ���� ��� �� ó��
    public bool TryGetMoveDirection(out Vector3 moveDir, out Quaternion planarRotation)
    {
        planarRotation = Quaternion.identity;
        moveDir = Vector3.zero;

        if (behaviour.GetInput(out NetworkInputData data))
        {
            Vector3 moveInput = new Vector3(data.moveAxis.x, 0, data.moveAxis.z).normalized;
            float yaw = Camera.main.transform.eulerAngles.y;
            planarRotation = Quaternion.Euler(0, yaw, 0);
            moveDir = planarRotation * moveInput;

            return moveInput.sqrMagnitude > 0.01f;
        }

        return false;
    }

    // ���� �Է� ó��
    public bool IsAttackPressed()
    {
        if (behaviour.GetInput(out NetworkInputData data))
        {
            return data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0);
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