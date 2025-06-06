using System;
using System.Collections.Generic;
using UnityEngine;



public abstract class StageManager<EStage> : MonoBehaviour where EStage : Enum
{
    protected Dictionary<EStage,BaseState<EStage>> states = new();

    protected BaseState<EStage> currentState;

    protected bool IsTransition = false;

    private void Start()
    {
        currentState.EnterState();
    }

    private void Update()
    {
        EStage nextStateKey = currentState.GetNextState();
        if (nextStateKey.Equals(currentState.StateKey))
        {
            currentState.UpdateState();
        }      
    }
    private void FixedUpdate()
    {
        EStage nextStateKey = currentState.GetNextState();
        if (nextStateKey.Equals(currentState.StateKey))
        {
            currentState.FixedUpdateState();
        }
        else
        {
            ChangeState(nextStateKey);
        }
    }



    public void ChangeState(EStage nextStateKey)
    {
        IsTransition= true;
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
