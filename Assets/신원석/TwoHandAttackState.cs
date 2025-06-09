//using UnityEngine;

//public class TwoHandAttackState : BaseState<PlayerStateMachine.PlayerState>
//{
//    public TwoHandAttackState(PlayerStateMachine.PlayerState key, Animator animator) : base(key, animator)
//    {

//    }

//    public override void EnterState()
//    {
//        animator.SetTrigger("IdleTrigger");
//    }
//    public override void ExitState() => Debug.Log("Exit Idle");
//    public override void UpdateState()
//    {
//        Debug.Log("Idle Updating...");
//        // 상태 전이 조건 확인 등
//    }

//    public override PlayerStateMachine.PlayerState GetNextState()
//    {
//        if (Input.GetKey(KeyCode.W))
//            return PlayerStateMachine.PlayerState.Move;

//        return PlayerStateMachine.PlayerState.Idle;
//    }

//    public override void OnTriggerEnter(Collider collider) { }
//    public override void OnTriggerExit(Collider collider) { }
//    public override void OnTriggerStay(Collider collider) { }

//    public override void FixedUpdateState(){ }
//}