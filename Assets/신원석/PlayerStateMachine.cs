using System;
using Unity.Cinemachine;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class PlayerStateMachine : StageManager<PlayerStateMachine.PlayerState>
{
    public enum PlayerState
    {
        Idle,
        Move,
        Switch,
        Jump,
        Attack,
        Roll,
    }

    Animator animator;
    [SerializeField]
    CinemachineCamera cinemachine;

    public bool isWeapon = false;

    public Transform groundCheck;
    public LayerMask groundMask;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        // 각 상태 객체 생성 및 등록
        states[PlayerState.Idle] = new IdleState(PlayerState.Idle,animator, this);
        states[PlayerState.Move] = new MoveState(PlayerState.Move, animator, this);
        states[PlayerState.Switch] = new WeaponSwitchState(PlayerState.Switch, animator, this);
        states[PlayerState.Attack] = new AttackState(PlayerState.Attack,animator, this);
        states[PlayerState.Roll] = new RollState(PlayerState.Roll, animator, this);
        states[PlayerState.Jump] = new JumpState(PlayerState.Jump, animator, this);

        // 초기 상태 설정
        currentState = states[PlayerState.Idle];
    }

    

}
