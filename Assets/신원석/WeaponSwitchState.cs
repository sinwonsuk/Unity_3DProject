using UnityEngine;
using UnityEngine.AI;

public class WeaponSwitchState : BaseState<PlayerStateMachine.PlayerState>
{
    PlayerStateMachine playerStateMachine;
    public WeaponSwitchState(PlayerStateMachine.PlayerState key, Animator animator,PlayerStateMachine playerStateMachine) : base(key, animator)
    {
        cameraController = Camera.main.GetComponent<CameraController>();

        this.playerStateMachine = playerStateMachine;

    }

    public override void EnterState()
    {
        animator.SetTrigger("KatanaEquip");
        playerStateMachine.isWeapon = true;

        EventBus<EquipWeaponEvent>.Raise(new EquipWeaponEvent(1));
    }
    public override void ExitState()
    {

    }
    public override void UpdateState()
    {
       
    }

    public override PlayerStateMachine.PlayerState GetNextState()
    {
       
        return PlayerStateMachine.PlayerState.Idle;
    }

    public override void OnTriggerEnter(Collider collider) { }
    public override void OnTriggerExit(Collider collider) { }
    public override void OnTriggerStay(Collider collider) { }

    public override void FixedUpdateState(){ }

    public override void LateUpdateState(){ }

    float moveAmount;
    CameraController cameraController;
    [SerializeField] float rotationSpeed = 500f;
    Quaternion targetRotation;
    Transform transform;
}
