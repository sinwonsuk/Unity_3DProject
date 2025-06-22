using Fusion;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



public abstract class StageManager<EStage> : NetworkBehaviour where EStage : Enum
{
    protected Dictionary<EStage, BaseState<EStage>> states = new();

    protected BaseState<EStage> currentState;

    protected bool IsTransition = false;

    private void Start()
    {
        currentState.EnterState();
    }

    private void Update()
    {

    }


    private void LateUpdate()
    {

            
    }

    public void ChangeState(EStage nextStateKey)
    {

        if (nextStateKey.Equals(currentState.StateKey))
        {
            return;
        }

        IsTransition = true;
        currentState.ExitState();
        currentState = states[nextStateKey];
        currentState.EnterState();
        IsTransition = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        currentState.OnTriggerEnter(other);
    }

    private void OnTriggerStay(Collider other)
    {
        currentState.OnTriggerStay(other);
    }

    private void OnTriggerExit(Collider other)
    {
        currentState.OnTriggerExit(other);
    }


}
