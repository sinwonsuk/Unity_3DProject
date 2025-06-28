using Cinemachine;
using Fusion;
using Fusion.Addons.SimpleKCC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;


public enum ItemState
{
    none,
    Sword,
    Harberd,
    Bow,
    FireMagic,
    IceMagic,
    ElectricMagic,
    Arrow,
    FireBall,
    IceBall,
    ElectricBall,
    HpPotion,
    StaminaPotion,
}

public enum PotionState
{
    none,
    HpPotion,
    StaminaPotion,
}

public class PlayerStateMachine : StageManager<PlayerStateMachine.PlayerState>
{
    [Networked] public PlayerState SyncedState { get; set; }
    [Networked] public bool comboAnimEnded { get; set; } = false;

    [Networked] public NetworkDictionary<PlayerRef, bool> hitMap { get; }
    [Networked] public int AttackCount { get; set; } = 0;
    [Networked] public bool isRoll { get; set; } = false;
    [Networked] public bool isAttack { get; set; } = true;
    [Networked] public bool isHit { get; set; } = true;
    [Networked] public int RollCount { get; set; } = 0;
    [Networked] public int HitCount { get; set; } = 0;
    [Networked] public TickTimer fireTimer { get; set; }
    [Networked] public float moveX { get; set;} = 0.0f;
    [Networked] public float moveZ { get; set; } = 0.0f;
    public float RollStaminaCost { get; set; } = 25f;
    public float JumpStaminaCost { get; set; } = 10.0f;
    [Networked] public float AttackStaminaCost { get; set; }
    [Networked] public float AttackCost { get; set; }
    public float MaxSpeed { get; set; } = 10.0f;
    public float CurrentSpeed { get; set; } = 5.0f;

    public NetworkMecanimAnimator NetAnim { get; set; }

    public HashSet<NetworkObject> hitSet { get; set; } = new();

    public float invulnDuration = 0.15f;

    private bool isDeath = false;

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
        Death,
        Portion,
    }
    public Action action;

    public WeaponsConfig weapons;

    private Animator animator;
    public PlayerCombat Combat { get; private set; }
    public AnimationHandler AnimHandler { get; private set; }
    public InputHandler inputHandler { get; private set; }

    public CameraManager cameraManager { get; private set; }
    public WeaponManager WeaponManager { get; private set; }

    public PlayerHealth health { get; set; }

    public PlayerStamina Stamina { get; set; }

    [SerializeField]
    float moveSpeed = 10.0f;
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
    Transform hitTransform;
    [SerializeField]
    LayerMask arrowHitMask;


    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = value;
    }

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
   
    bool _isInitialized = false;

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
        states[PlayerState.Death] = new DeathState(PlayerState.Death, this);
        states[PlayerState.Portion] = new PortionState(PlayerState.Portion, this);

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
        Stamina = GetComponent<PlayerStamina>();
        action = adad;

        if (Object.HasStateAuthority)
        {
            PlayerRef me = Object.InputAuthority;
            WeaponManager.MagicInitialize(HandSide.Right, me);
        }

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
     
               //MoveAndRotate(inputHandler.GetNetworkInputData());

        }
    }

    [Networked] public ItemState itemState { get; set; }
    [Networked] public PlayerRef owner { get; set; }

    [Networked] public ItemClass ItemClass { get; set; }

    public void WeaponChange(WeaponChange weaponChange)
    {

        if (Object.InputAuthority != weaponChange.inf2)
            return;

        owner = Object.InputAuthority;
        itemState = weaponChange.state;
        ItemClass = (ItemClass)weaponChange.num;
        BroadcastIdleEvent(PlayerState.Switch);

    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_ChangeWeaponAni(ItemState rollCount, RpcInfo info = default)
    {
        AnimHandler.ChangeWeapon((ItemState)rollCount);
    }

    public void MoveAndRotate(NetworkInputData data)
    {
        if(SyncedState == PlayerState.Death)
        {
            return;
        }


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


    // 1) 클라 → 호스트: 매직 풀에서 하나 꺼내 달라고 요청
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestMagic(ItemState state, HandSide dir, RpcInfo info = default)
    {
        NetworkObject magic = null;

        if (ItemState.IceMagic == state)
        {
            magic = WeaponManager.GetIceMagicPool();
        }
        if (ItemState.FireMagic == state)
        {
            magic = WeaponManager.GetFireMagicPool();
        }
        if (ItemState.ElectricMagic == state)
        {
            magic = WeaponManager.GetElectricMagicPool();
        }

        WeaponManager.CurrentMagicId = magic.Id;   // ID 저장                                                        
        RPC_ConfirmMagic(magic.Id);
    }

    // 2) 호스트 → 클라: (옵션) 누구에게 어떤 오브젝트인지 알려주고 싶을 때
    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
    void RPC_ConfirmMagic(NetworkId magicId, RpcInfo info = default)
    {
        // 이 RPC 는 요청을 보낸 클라이언트 쪽에서 실행됨
        WeaponManager.CurrentMagicId = magicId;
    }

    // 3) 클라 → 호스트: 이제 발사할 때
    public void SetShootMagicObject(Vector3 targetPos, ItemState state)
    {
        if (Object.HasInputAuthority)
        {
            // magicId 는 WeaponManager.CurrentMagicId 에 저장돼 있다고 가정
            RPC_RequestShoot(targetPos, state, WeaponManager.CurrentMagicId);
        }
    }
    public void SetShootArrowObject(Vector3 targetPos, ItemState state)
    {
        if (Object.HasInputAuthority)
        {
            RPC_RequestShoot(targetPos, state, WeaponManager.Arrow.Id);
        }
    }

    // 4) 클라 → 호스트: 실제 발사 처리
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    void RPC_RequestShoot(Vector3 targetPos, ItemState state, NetworkId magicId, RpcInfo info = default)
    {
        // StateAuthority(호스트)에서 실행
        var netObj = Runner.FindObject(magicId);
        if (netObj == null) return;

        if(ItemState.Arrow == state)
        {
            var shot = netObj.GetComponent<Arrow>();
            shot.ArrowShoot(targetPos);
        }
        else
        {            
            var shot = netObj.GetComponent<ShootObj>();
            shot.ArrowShoot(targetPos,ItemClass);
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

        if(health.currentHp <= 0 && isDeath ==false)
        {
            BroadcastIdleEvent(PlayerState.Death);
            currentState.ExitState();
            currentState = states[PlayerState.Death];
            currentState.EnterState();
            isDeath = true;
            return;
        }

        MoveAndRotate(inputHandler.GetNetworkInputData());

       
        if (Object.HasStateAuthority)
        {
            var next = currentState.GetNextState();
            if (next != currentState.StateKey)
            {
                SyncedState = next;
            }        
        }

        if (SyncedState != currentState.StateKey)
        {
            currentState.ExitState();
            currentState = states[SyncedState];
            currentState.EnterState();
        }

        currentState.FixedUpdateState();


    }
    public void adad()
    {
        if (currentState is BaseState<PlayerState> attackState && comboAnimEnded == true)
        {
            attackState.OnAnimationEvent();
            comboAnimEnded = false;
        }

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

    public void StopRoll() => isRoll = true;
    public void startRoll() => isRoll = false;
    public void SetIsAttackTrue() => isAttack = true;
    public void SetIsAttackFalse() => isAttack = false;

    public void SetIsHitTrue() => isHit = true;
    public void SetIsHitFalse() => isHit = false;

}
