using System.Collections;
using UnityEngine;

public class AttackState : BaseState<PlayerStateMachine.PlayerState>
{
    PlayerStateMachine stateMachine;

    private float comboInputWindow = 0.5f; // 콤보 입력을 기다리는 시간
    private float comboTimer = 0f;
    private bool isWaitingForCombo = false;

    //private int AttackCount = 0;
    private bool nextAttackQueued = false;

    public AttackState(PlayerStateMachine.PlayerState key, Animator animator, PlayerStateMachine stateMachine ) : base(key, animator)
    {
        this.stateMachine = stateMachine;
    }

    public override void EnterState()
    {
        stateMachine.Combat.StartAttack();
    }
    public override void ExitState()
    {
        stateMachine.SetIsAttackFalse();
        animator.SetBool("RunAttack", false);
        animator.SetBool("Attack", false);
    }

    public override void UpdateState()
    {

      
       
    }
    public override void FixedUpdateState() 
    {
        if (!stateMachine.HasInputAuthority)
            return;

        // 카메라의 현재 수평(Y) 회전만 가져와서 적용
        float yaw = Camera.main.transform.eulerAngles.y;
        Quaternion planarRotation = Quaternion.Euler(0, yaw, 0);
        stateMachine.transform.rotation = planarRotation;

        stateMachine.AttackMove();
    }

    public override PlayerStateMachine.PlayerState GetNextState()
    {       
        return PlayerStateMachine.PlayerState.Attack;
    }
    public void ChangeRotate()
    {

    }
    public override void OnTriggerEnter(Collider collider) { }
    public override void OnTriggerExit(Collider collider) { }
    public override void OnTriggerStay(Collider collider) { }

    public override void LateUpdateState(){ }

    public override void OnAttackAnimationEnd()
    {
        stateMachine.Combat.OnAnimationEnd();
    }

    int hashAttackCount = Animator.StringToHash("AttackCount");


    public int AttackCount
    {
        get => animator.GetInteger(hashAttackCount);
        set => animator.SetInteger(hashAttackCount, value);
    }

    float rotationSpeed = 100.0f;
    Quaternion targetRotation;

}