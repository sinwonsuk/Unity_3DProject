using Fusion;
using Cinemachine;
using UnityEngine;
using System;
using Fusion.Addons.SimpleKCC;
using System.Collections.Generic;
using System.Collections;


public enum ItemState
{
    none,
    Sword,
    Harberd,
    Bow,
    FireMagic,
    IceMagic,
    ElectricMagic,
    Position,
    Arrow,
    FireBall,
    IceBall,
    ElectricBall,
}


public class PlayerStateMachine : StageManager<PlayerStateMachine.PlayerState>
{
    [Networked] public PlayerState SyncedState { get; set; }
    [Networked] public bool comboAnimEnded { get; set; } = false;

    [Networked] public int AttackCount { get; set; } = 0;
    [Networked] public bool isRoll { get; set; } = false;
    [Networked] public bool isAttack { get; set; } = true;

    [Networked] public bool isHit { get; set; } = true;
    [Networked] public int RollCount { get; set; } = 0;

    [Networked] public int HitCount { get; set; } = 0;

    [Networked] public float gatherAttack {get;set;}= 0;
    public NetworkMecanimAnimator NetAnim { get; set; }

    public HashSet<NetworkObject> hitSet { get; set; } = new();

    int a = 0;
    [Networked] public bool _canBeHit { get; set; } = true;

    public float invulnDuration = 0.15f;

    public enum PlayerState
    {
        Idle,
        Move,
        Switch,
        Jump,
        Attack,
        Roll,
        BowAttack,
        Magic,
        Hit,
    }
    public Action action;

    public WeaponsConfig weapons;
    public ItemState Item { get; set; } = ItemState.none;

    private Animator animator;
    public PlayerCombat Combat { get; private set; }
    public AnimationHandler AnimHandler { get; private set; }
    public InputHandler inputHandler { get; private set; }

    public CameraManager cameraManager { get; private set; }
    public WeaponManager WeaponManager { get; private set; }

    public PlayerHealth health { get; set; }


    [SerializeField]
    float moveSpeed = 5.0f;
    [SerializeField]
    float rotationSpeed = 2.0f;
    [SerializeField]
    Transform cameraFollow;
    [SerializeField]
    float rollSpeed = 15.0f;
    [SerializeField]
    float attackSpeed = 2.0f;
    [SerializeField]
    Transform rightHandTransform;
    [SerializeField]
    Transform leftHandTransform;
    [SerializeField]
    LayerMask arrowHitMask;

    public float AttackSpeed
    {
        get => attackSpeed;
        set => attackSpeed = value;
    }
    public Transform RightHandTransform
    {
        get => rightHandTransform;
        set => rightHandTransform = value;
    }
    public Transform LeftHandTransform
    {
        get => leftHandTransform;
        set => leftHandTransform = value;
    }
    public Transform CameraFollow
    {
        get => cameraFollow;
        set => cameraFollow = value;
    }
    public LayerMask ArrowHitMask
    {
        get => arrowHitMask;
        set => arrowHitMask = value;
    }
    public CinemachineVirtualCamera Cam { get; set; }

    public Transform groundCheck;
    public LayerMask groundMask;
   [Networked] public bool IsWeapon { get; set; } = false;
   
    public SimpleKCC playerController {  get; private set; }
   
    public Vector3 rootMotionDelta { get; set; }
    public Quaternion rootMotionRotation { get; set; }


    public bool nextAttackQueued { get; set; } = false;
 
    bool _isInitialized = false;

    public int Hp { get; set; }

    public override void Spawned()
    {
        NetAnim = GetComponent<NetworkMecanimAnimator>();
        animator = GetComponent<Animator>();
        playerController = GetComponent<SimpleKCC>();
        


        playerController.SetGravity(-9.8f);

        states[PlayerState.Idle] = new IdleState(PlayerState.Idle, this);
        states[PlayerState.Move] = new MoveState(PlayerState.Move, this);
        states[PlayerState.Switch] = new WeaponSwitchState(PlayerState.Switch, this);
        states[PlayerState.Attack] = new AttackState(PlayerState.Attack, this);
        states[PlayerState.Roll] = new RollState(PlayerState.Roll, animator, this);
        states[PlayerState.BowAttack] = new BowState(PlayerState.BowAttack, animator, this);
        states[PlayerState.Jump] = new JumpState(PlayerState.Jump, this);
        states[PlayerState.Magic] = new MagicAttackState(PlayerState.Magic, animator, this);
        states[PlayerState.Hit] = new HitState(PlayerState.Hit, this);

        if (Object.HasStateAuthority)
            SyncedState = PlayerState.Idle;

        currentState = states[SyncedState];

        if (Object.HasInputAuthority)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;
            GameObject camObj = GameObject.FindGameObjectWithTag("VirtualCam2");
            Cam = camObj.GetComponent<CinemachineVirtualCamera>();
            Cam.Follow = cameraFollow;
            Cam.LookAt = cameraFollow;

        }

        inputHandler = new InputHandler(this, CameraFollow);
        Combat = new PlayerCombat(this);
        AnimHandler = new AnimationHandler(NetAnim,this);
        WeaponManager = GetComponent<WeaponManager>();
        WeaponManager.Init(weapons, rightHandTransform, leftHandTransform, Runner, this);
        cameraManager = new CameraManager(Cam);
        health = GetComponent<PlayerHealth>();

        action = adad;
        _isInitialized = true;
    }
  
    private void OnEnable()
    {
        EventBus<WeaponChange>.OnEvent += WeaponChange;
    }
    private void OnDisable()
    {
        EventBus<WeaponChange>.OnEvent -= WeaponChange;
    }
    public void OnAnimationEnd()
    {
        if (currentState is BaseState<PlayerState> attackState)
        {
            if (!Object.HasStateAuthority) return;
            comboAnimEnded = true;

        }
    }
    public void MoveInput()
    {
        if (inputHandler.IsMove() == true)
        {
            if (Object.HasInputAuthority && !Object.HasStateAuthority)
            {              
               MoveAndRotate(inputHandler.GetNetworkInputData());
            }
            else if (Object.HasStateAuthority)
            {
               MoveAndRotate(inputHandler.GetNetworkInputData());
            }
        }
    }

    public void WeaponChange(WeaponChange weaponChange)
    {
        if (Object.HasStateAuthority)
        {
            PlayerRef me = Object.InputAuthority;
            SetWeapon(true);
            WeaponManager.RequestEquip(weaponChange.state, HandSide.Right, me);
            OnAttackEndEvent();
        }
        AnimHandler.ChangeWeapon(weaponChange.state);
    }


    public void MoveAndRotate(NetworkInputData data)
    {
        Vector3 moveInput = data.direction.normalized;
        Quaternion planarRot = Quaternion.Euler(0, data.CameraRotateY, 0);
        Vector3 moveDir = planarRot * moveInput;

        playerController.Move(moveDir * moveSpeed);

        Rotation(data);
    }

    public void Rotation(NetworkInputData data)
    {
        float targetYaw = data.CameraRotateY;  


        float currentYaw = playerController.TransformRotation.eulerAngles.y;

        float smoothing = 10f; 
        float smoothedYaw = Mathf.LerpAngle(currentYaw, targetYaw, smoothing * Runner.DeltaTime);

        playerController.SetLookRotation(0, smoothedYaw);
    }


    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_BroadcastState(PlayerState next, RpcInfo info = default)
    {
        if (!Object.HasStateAuthority) return;

        if (SyncedState == next) return;
        SyncedState = next;
    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetRollCount(int rollCount, RpcInfo info = default)
    {
        RollCount = rollCount;
    }


    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void Rpc_SetWeapon(bool hasWeapon, RpcInfo info = default)
    {
        if (!Object.HasStateAuthority) return;

        IsWeapon = hasWeapon; 
    }



    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void Rpc_EndAttack(RpcInfo info = default)
    {
        AnimHandler.SetAttackBool(false);

    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_RequestShoot(Vector3 targetPos, ItemState State, RpcInfo info = default)
    {
        ShootObj arrow = null;

        if (State == ItemState.IceMagic)
        {
            arrow = WeaponManager.Magic.GetComponent<ShootObj>();
        }
        if (State == ItemState.FireMagic)
        {
            arrow = WeaponManager.Magic.GetComponent<ShootObj>();
        }
        if (State == ItemState.ElectricMagic)
        {
            arrow = WeaponManager.Magic.GetComponent<ShootObj>();
        }
        if (State == ItemState.Arrow)
        {
            arrow = WeaponManager.Arrow.GetComponent<ShootObj>();
        }

        Vector3 dir = targetPos;

        arrow.Shoot(dir);
    }
    public void SetShootObject(Vector3 targetPos,ItemState State)
    {
        if (Object.HasInputAuthority)
        {
            RPC_RequestShoot(targetPos, State);
        }
        else if (Object.HasStateAuthority)
        {
            ShootObj arrow = null;

            if (State == ItemState.IceMagic)
            {
                arrow = WeaponManager.Magic.GetComponent<ShootObj>();
            }
            if (State == ItemState.FireMagic)
            {
                arrow = WeaponManager.Magic.GetComponent<ShootObj>();
            }
            if (State == ItemState.ElectricMagic)
            {
                arrow = WeaponManager.Magic.GetComponent<ShootObj>();
            }
            if (State == ItemState.Arrow)
            {
                arrow = WeaponManager.Arrow.GetComponent<ShootObj>();
            }
                
            Vector3 dir = targetPos;

            arrow.Shoot(dir);
        }
    }

 

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_ClearHitSet()
    {
        hitSet.Clear();
    }



    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_PlayHitAnimation(int newHitCount)
    {
        AnimHandler.SetHitCount(newHitCount);        
    }

    public void PlayHitAnimation(int newHitCount)
    {

        if(Object.HasStateAuthority)
        {
            RPC_PlayHitAnimation(newHitCount);
        }
    }


    public void ClearHitSet()
    {
        if (Object.HasInputAuthority)
            RPC_ClearHitSet();
        if (Object.HasStateAuthority)
            hitSet.Clear();
    }

    public void SetWeapon(bool hasWeapon)
    {
        if (Object.HasInputAuthority)
        {
            Rpc_SetWeapon(hasWeapon);
        }
        if(Object.HasStateAuthority)
        {
            IsWeapon = hasWeapon;  
        }

    }

    public void BroadcastIdleEvent(PlayerState nextStateKey)
    {
        if (Object.HasStateAuthority)
        {
            SyncedState = nextStateKey;
        }
        else if(Object.HasInputAuthority)
        {
            RPC_BroadcastState(nextStateKey);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!_isInitialized) return;

        Debug.Log(health.currentHp);


        //string who = Object.HasInputAuthority && Runner.LocalPlayer == Object.InputAuthority
        //? "�� �÷��̾�"
        //: "�ٸ� �÷��̾�";

        //Debug.Log($"[{who}] ObjID={Object.Id} InputAuth={Object.HasInputAuthority} StateAuth={Object.HasStateAuthority} SyncedState={SyncedState}");

        if (Object.HasStateAuthority)
        {
            var next = currentState.GetNextState();
            if (next != currentState.StateKey)
            {
                SyncedState = next;
            }        
        }
        // (3) ����: ����ȭ�� ���� �ݿ�
        if (SyncedState != currentState.StateKey)
        {
            currentState.ExitState();
            currentState = states[SyncedState];
            currentState.EnterState();
        }
        // (4) ����: ���º� �ൿ ����
        currentState.FixedUpdateState();

        // (5) �̺�Ʈ ����

    }
    public void adad()
    {
        if (currentState is BaseState<PlayerState> attackState && comboAnimEnded == true)
        {
            attackState.OnHitAnimationEvent();
            comboAnimEnded = false;
        }

    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_PlayHit()
    {
        NetAnim.Animator.SetTrigger("HitTrigger");
        SyncedState = PlayerState.Hit;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_ToggleWeaponCollider(bool enable)
    {
        WeaponManager.currentWeapon.GetComponent<MeshCollider>().enabled = enable;
    }

    public void OnAttackStartEvent()
    {
        if (!Object.HasStateAuthority) return;    
        RPC_ToggleWeaponCollider(true);
    }

    public void OnAttackEndEvent()
    {
        if (!Object.HasStateAuthority) return;   
        RPC_ToggleWeaponCollider(false);

    }




    //public void ComboAttackInput()
    //{
    //    if (IsWeapon == false)
    //        return;



    //    if (playerStateMachine.inputHandler.IsAttackPressed() && playerStateMachine.Object.HasInputAuthority &&  playerStateMachine.AnimHandler.WeaponCount != (int)ItemState.Bow && playerStateMachine.AnimHandler.WeaponCount != 4)
    //    {
    //        if (Object.HasStateAuthority)
    //        {
    //            SyncedState = PlayerState.Attack;
    //        }
    //        else
    //        {
    //            RPC_BroadcastState(PlayerState.Attack);
    //        }

    //        playerStateMachine.RPC_BroadcastState(PlayerState.Attack);
    //        return;
    //    }



    //    //if (inputHandler.IsAttackPressed() == true && Object.HasInputAuthority && AnimHandler.WeaponCount != 4 && AnimHandler.WeaponCount != ItemState.Bow)
    //    //{
    //    //    if (Object.HasStateAuthority)
    //    //    {

    //    //        SyncedState = PlayerState.Attack;
    //    //    }
    //    //    else
    //    //    {

    //    //        RPC_BroadcastState(PlayerState.Attack);
    //    //    }

    //    //}
    //}

    //public bool DashAttackInput()
    //{
    //    if (IsWeapon == false)
    //        return false;

    //    if (inputHandler.IsDashAttackPressed())
    //    {
    //        NetAnim.Animator.SetBool("RunAttack", true);
    //        RPC_BroadcastState(PlayerState.Attack);
    //        return true;
    //    }

    //    return false;
    //}
    public IEnumerator InvulnCoroutine()
    {
        yield return new WaitForSeconds(invulnDuration);
        _canBeHit = true;
    }
    public override void Render()
    {
        AnimHandler.SetHitCount(HitCount);
    }

    public void StopRoll() => isRoll = true;
    public void startRoll() => isRoll = false;
    public void SetIsAttackTrue() => isAttack = true;
    public void SetIsAttackFalse() => isAttack = false;

    public void SetIsHitTrue() => isHit = true;
    public void SetIsHitFalse() => isHit = false;

}
