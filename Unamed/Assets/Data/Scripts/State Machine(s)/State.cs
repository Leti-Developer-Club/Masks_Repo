using UnityEngine;

public abstract class State
{
    public StateMachine stateMachine;

    public virtual void OnEnter() 
    {
    }

    public virtual void OnUpdate()
    {

    }

    public virtual void OnExit() 
    {
        
    }
}
