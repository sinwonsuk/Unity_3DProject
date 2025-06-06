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

    private void Awake()
    {
        animator = GetComponent<Animator>();

        // �� ���� ��ü ���� �� ���
        states[PlayerState.Idle] = new IdleState(PlayerState.Idle,animator);
        states[PlayerState.Move] = new MoveState(PlayerState.Move, animator, this);
        states[PlayerState.Switch] = new WeaponSwitchState(PlayerState.Switch, animator, transform);
        states[PlayerState.Attack] = new AttackState(PlayerState.Attack,animator, this);
        states[PlayerState.Roll] = new RollState(PlayerState.Roll, animator, this);
        // �ʱ� ���� ����
        currentState = states[PlayerState.Idle];


    }
}
