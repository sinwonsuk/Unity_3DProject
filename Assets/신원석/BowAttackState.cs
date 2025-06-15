using UnityEngine;
using static UnityEditor.Recorder.OutputPath;

public class BowState : BaseState<PlayerStateMachine.PlayerState>
{
    PlayerStateMachine playerStateMachine;
    Transform rope;
    float ropePosX;
    float rotationSpeed = 300.0f;

    public float ySmoothT = 0.1f;

    Quaternion targetRotation;
    public BowState(PlayerStateMachine.PlayerState key, Animator animator, PlayerStateMachine stateMachine) : base(key, animator)
    {
        this.playerStateMachine = stateMachine;
      
    }

    public override void EnterState()
    {
        playerStateMachine.AnimHandler.SetAttackBool(true);
        rope = playerStateMachine.WeaponManager.GetCurrentWeapon().GetComponent<Bow>().Rope.transform;
    }
    public override void ExitState()
    {
        playerStateMachine.AnimHandler.SetAttackBool(false);

        shoot = false;
        //playerStateMachine.Cam.Priority = 9;
    }

    public override void UpdateState()
    {

    }



    public override void FixedUpdateState()
    {

        if (playerStateMachine.GetInput(out NetworkInputData data))
        {
            Quaternion quaternion = Quaternion.Euler(0, data.CameraRotateY, 0);

            playerStateMachine.playerController.Rotate(quaternion);

        }



        if (playerStateMachine.inputHandler.IsRightAttackPressed() && Input.GetMouseButton(0) && gatherAttack ==1 && shoot == false)
        {
            // 활 발사도 만들어야함 
             shoot = true;
            playerStateMachine.AnimHandler.ShootBowWeapon();           
        }
        else if (Input.GetMouseButton(1) && shoot ==false)
        {
            //playerStateMachine.Cam.Priority = 11;


            gatherAttack = Mathf.MoveTowards(gatherAttack, 1.0f, playerStateMachine.Runner.DeltaTime);

            ropePosX = Mathf.MoveTowards(ropePosX, 0.8f, playerStateMachine.Runner.DeltaTime*1.2f);


            

            rope.localPosition = new Vector3(ropePosX, rope.localPosition.y, rope.localPosition.z);


            playerStateMachine.AnimHandler.ChangeBowWeaponState(gatherAttack);
            playerStateMachine.AnimHandler.ChangeBowWeaponState(gatherAttack);
        }
        else
        {
            ropePosX = Mathf.MoveTowards(ropePosX, 0.19f, playerStateMachine.Runner.DeltaTime * 8.0f);

            rope.localPosition = new Vector3(ropePosX, rope.localPosition.y, rope.localPosition.z);

            gatherAttack = Mathf.MoveTowards(gatherAttack, 0, playerStateMachine.Runner.DeltaTime * 2.0f);
            playerStateMachine.AnimHandler.ChangeBowWeaponState(gatherAttack);
        }



        if (gatherAttack == 0)
           // playerStateMachine.ChangeState(PlayerStateMachine.PlayerState.Idle);
        return;


    }
    public override void LateUpdateState() 
    {
        if (!playerStateMachine.HasInputAuthority)
            return;

        // 부드러운 회전 보간
        playerStateMachine.transform.rotation = Quaternion.RotateTowards(
            playerStateMachine.transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    public override PlayerStateMachine.PlayerState GetNextState()
    {
        return PlayerStateMachine.PlayerState.BowAttack;
    }
    public void ChangeRotate()
    {

    }
    public override void OnTriggerEnter(Collider collider) { }
    public override void OnTriggerExit(Collider collider) { }
    public override void OnTriggerStay(Collider collider) { }

    

    public override void OnAttackAnimationEnd()
    {
        //stateMachine.Combat.OnAnimationEnd();
    }


    bool shoot = false;
    float gatherAttack = 0;

 

}