
using Fusion;
using UnityEngine;
using static PlayerStateMachine;
using static Unity.Collections.Unicode;


public class BowState : BaseState<PlayerStateMachine.PlayerState>
{
    PlayerStateMachine playerStateMachine;
    Transform rope;
    float ropePosX;
    Coroutine zoomRoutine;

    public float ySmoothT = 0.1f;

    Quaternion targetRotation;

    NetworkObject arrow;
    Vector3 targetPos;


    public BowState(PlayerStateMachine.PlayerState key, Animator animator, PlayerStateMachine stateMachine) : base(key)
    {
        this.playerStateMachine = stateMachine;
      
    }

    public override void EnterState()
    {

        playerStateMachine.AnimHandler.SetAttackBool(true);
        gatherAttack = 0;

        rope = playerStateMachine.WeaponManager.currentWeapon.GetComponent<Bow>().Rope.transform;

        if (playerStateMachine.Object.HasStateAuthority)
        {
            PlayerRef me = playerStateMachine.Object.InputAuthority;
            playerStateMachine.WeaponManager.RequestArrow(ItemState.Arrow, HandSide.Right, me);
            arrow = playerStateMachine.WeaponManager.Arrow;
        }

        playerStateMachine.cameraManager.isCameraCheck = false;

        if (playerStateMachine.Object.HasInputAuthority)
            zoomRoutine = playerStateMachine.StartCoroutine(playerStateMachine.cameraManager.ZoomDistance(1f));

    }
    public override void ExitState()
    {
        playerStateMachine.cameraManager.isCameraCheck = false;

        if (playerStateMachine.Object.HasInputAuthority)
        {
            if (zoomRoutine != null) 
                playerStateMachine.StopCoroutine(zoomRoutine);

            zoomRoutine = playerStateMachine.StartCoroutine(playerStateMachine.cameraManager.ZoomDistance(2f));
        }
        rope.localPosition = new Vector3(0.19f, rope.localPosition.y, rope.localPosition.z);
        playerStateMachine.AnimHandler.SetAttackBool(false);
        shoot = false;
        arrow = null;
    }

    public override void FixedUpdateState()
    {
       if (!playerStateMachine.Object.HasInputAuthority && !playerStateMachine.Runner.IsForward)
            return;


        playerStateMachine.AnimHandler.ChangeBowWeaponState(gatherAttack);

        if (playerStateMachine.GetInput(out NetworkInputData data))
            playerStateMachine.Rotation(data);


        float attackStamina = playerStateMachine.AttackStaminaCost;
        float CutrrentStanmina = playerStateMachine.Stamina.currentStamina;


        if ( playerStateMachine.inputHandler.IsRightAttackPressed() &&
             playerStateMachine.inputHandler.IsAttackPressed() && gatherAttack ==1 
             && shoot == false && playerStateMachine.cameraManager.isCameraCheck ==true && attackStamina < CutrrentStanmina)
        {
             Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
  
            if (Physics.Raycast(ray, out var hit, 999f, playerStateMachine.ArrowHitMask))
                targetPos = hit.point;
            else
            {
                float fallbackDist = 50f;
                targetPos = ray.origin + ray.direction * fallbackDist;
            }           
            playerStateMachine.Stamina.ConsumeStaminaOnServer(attackStamina);

            playerStateMachine.SetShootArrowObject(targetPos,ItemState.Arrow);

            playerStateMachine.AnimHandler.ShootBowWeapon();
            shoot = true;
        }
        else if (playerStateMachine.inputHandler.IsRightAttackPressed() && shoot ==false)
        {
         
            gatherAttack = Mathf.MoveTowards(gatherAttack, 1.0f, playerStateMachine.Runner.DeltaTime);

            ropePosX = Mathf.MoveTowards(ropePosX, 0.8f, playerStateMachine.Runner.DeltaTime*1.2f);
       
            rope.localPosition = new Vector3(ropePosX, rope.localPosition.y, rope.localPosition.z);
        }
        else
        {
            ropePosX = Mathf.MoveTowards(ropePosX, 0.19f, playerStateMachine.Runner.DeltaTime * 8.0f);
            rope.localPosition = new Vector3(ropePosX, rope.localPosition.y, rope.localPosition.z);
            gatherAttack = Mathf.MoveTowards(gatherAttack, 0, playerStateMachine.Runner.DeltaTime * 2.0f);
        }
        if (gatherAttack == 0)
        {
            playerStateMachine.Runner.Despawn(playerStateMachine.WeaponManager.Arrow);
            playerStateMachine.BroadcastIdleEvent(PlayerState.Idle);
            return;
        }

    }

    public override PlayerStateMachine.PlayerState GetNextState()
    {
        return PlayerStateMachine.PlayerState.BowAttack;
    }
    public void ChangeRotate()
    {

    }
    public override void OnTriggerEnter(Collider collider)
    {
        // 1) 호스트에서만 충돌 처리
        if (!playerStateMachine.Object.HasStateAuthority)
            return;

        // 2) Weapon 네트워크 오브젝트 가져오기
        var weaponNetObj = collider.GetComponent<NetworkObject>();
        if (weaponNetObj == null || !collider.CompareTag("Weapon"))
            return;


        // 3) Weapon의 입력 권한자가 이 플레이어와 같다면 스킵
        if (weaponNetObj.InputAuthority == playerStateMachine.Object.InputAuthority)
            return;

        int attack = weaponNetObj.gameObject.GetComponent<WeaponNetworkObject>().weaponInfoConfig.Attack;

        playerStateMachine.health.RequestDamage(attack);

        playerStateMachine.BroadcastIdleEvent(PlayerState.Hit);
    }
    public override void OnTriggerExit(Collider collider) { }
    public override void OnTriggerStay(Collider collider) { }

    

    public override void OnAnimationEvent()
    {
        //stateMachine.Combat.OnAnimationEnd();
    }


    bool shoot = false;
    float gatherAttack = 0;

}