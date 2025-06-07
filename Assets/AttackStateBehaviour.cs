using UnityEngine;

public class AttackStateBehaviour : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<PlayerStateMachine>().ChangeState(PlayerStateMachine.PlayerState.Idle);
    }
}