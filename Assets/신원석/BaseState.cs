using System;
using UnityEngine;

//public enum EState
//{
//    Idle,
//    Move,
//    Attack,
//}

public abstract class BaseState<EState> where EState : Enum
{
    protected Animator animator;
    public BaseState(EState key, Animator animator)
    {
        StateKey = key;
        this.animator = animator;
    }

    public EState StateKey { get; set; }

    public abstract void EnterState();
    public abstract void ExitState();
    public abstract void UpdateState();
    public abstract EState GetNextState();
    public abstract void OnTriggerEnter(Collider collider);
    public abstract void OnTriggerExit(Collider collider);
    public abstract void OnTriggerStay(Collider collider);


}