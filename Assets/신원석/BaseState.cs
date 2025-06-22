using System;
using UnityEngine;



public abstract class BaseState<EState> where EState : Enum
{
    public BaseState(EState key)
    {
        StateKey = key;
    }

    public EState StateKey { get; set; }

    public abstract void EnterState();
    public abstract void ExitState();
    public abstract void FixedUpdateState();
    public abstract void OnAttackAnimationEnd();
    public abstract EState GetNextState();
    public abstract void OnTriggerEnter(Collider collider);
    public abstract void OnTriggerExit(Collider collider);
    public abstract void OnTriggerStay(Collider collider);


}