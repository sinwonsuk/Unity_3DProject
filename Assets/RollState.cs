using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;

public enum ERollState
{
    Left,
    Forward,
    Backward,
    Right,
}

public class RollState : BaseState<PlayerStateMachine.PlayerState>
{
    PlayerStateMachine PlayerStateMachine;

    public RollState(PlayerStateMachine.PlayerState key, Animator animator, PlayerStateMachine playerStateMachine) : base(key, animator)
    {
        this.PlayerStateMachine = playerStateMachine;


        cameraController = Camera.main.GetComponent<CameraController>();
    }

    public override void EnterState()
    {
        animator.SetTrigger("RollTrigger");      
    }
    public override void ExitState()
    {
       
    }
    public override void UpdateState()
    {     
        //AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        //if (stateInfo.normalizedTime >= 0.5f )
        //{
        //    //animator.SetInteger("RollCount", 0);
        //    PlayerStateMachine.ChangeState(PlayerStateMachine.PlayerState.Idle);
        //    return;
        //}
    }

    public override void FixedUpdateState()
    {
              
    }



    public override PlayerStateMachine.PlayerState GetNextState()
    {
        return PlayerStateMachine.PlayerState.Roll;
    }

    public override void OnTriggerEnter(Collider collider) { }
    public override void OnTriggerExit(Collider collider) { }
    public override void OnTriggerStay(Collider collider) { }

    public override void LateUpdateState(){ }

    float moveSpeed = 2.0f;


    CameraController cameraController;
    CinemachineCamera camera;

    [SerializeField] float rotationSpeed = 500f;
    Quaternion targetRotation;
}
