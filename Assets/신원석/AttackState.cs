using System.Collections;
using UnityEngine;

public class AttackState : BaseState<PlayerStateMachine.PlayerState>
{
    PlayerStateMachine stateMachine;

    public AttackState(PlayerStateMachine.PlayerState key, Animator animator, PlayerStateMachine stateMachine ) : base(key, animator)
    {
        this.stateMachine = stateMachine;
    }

    public override void EnterState()
    {



        AttackCount = 1;
        animator.SetBool("Attack", true);
        nextAttackQueued = false;

        stateMachine.StartCoroutine(DelayInput());

    }
    public override void ExitState()
    {
        AttackCount = 0;
        nextAttackQueued = false;

        coroutine = null;
        stateMachine.StopCoroutine(DelayInput());

    }

    IEnumerator DelayInput()
    {
        

        yield return new WaitForSeconds(0.3f);

        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (AttackCount < 4)
                {
                    nextAttackQueued = true;                  
                }
            }

            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.normalizedTime >= 1f && !animator.IsInTransition(0))
            {
                if (nextAttackQueued && AttackCount < 4)
                {
                    AttackCount++;
                    animator.SetBool("Attack", true);
                    nextAttackQueued = false;                   
                }
                else
                {
                    animator.SetBool("Attack", false);
                    AttackCount = 0;
                    stateMachine.ChangeState(PlayerStateMachine.PlayerState.Idle);
                    yield break;
                }

                
            }
            yield return null; 
        }
    }


    public override void UpdateState()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        float moveAmount = Mathf.Clamp01(Mathf.Abs(h) + Mathf.Abs(v));

        Vector3 moveInput = new Vector3(h, 0, v).normalized;

        // 카메라의 현재 수평(Y) 회전만 가져와서 적용
        float yaw = Camera.main.transform.eulerAngles.y;
        Quaternion planarRotation = Quaternion.Euler(0, yaw, 0);


        Vector3 moveDir = planarRotation * moveInput;

        targetRotation = Quaternion.LookRotation(moveDir);

        stateMachine.transform.rotation = Quaternion.RotateTowards(stateMachine.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
       
    }
    public override void FixedUpdateState() { }

    public override PlayerStateMachine.PlayerState GetNextState()
    {
        


        return PlayerStateMachine.PlayerState.Attack;
    }

    public override void OnTriggerEnter(Collider collider) { }
    public override void OnTriggerExit(Collider collider) { }
    public override void OnTriggerStay(Collider collider) { }


    Coroutine coroutine;

    int hashAttackCount = Animator.StringToHash("AttackCount");

    bool nextAttackQueued = false;
    public int AttackCount
    {
        get => animator.GetInteger(hashAttackCount);
        set => animator.SetInteger(hashAttackCount, value);
    }

    float rotationSpeed = 100.0f;
    Quaternion targetRotation;

}