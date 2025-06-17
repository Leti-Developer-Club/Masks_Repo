using UnityEngine;

 public class StateMachine : MonoBehaviour
{
    State CurrentState;

    void Update()
    {
        if (CurrentState != null)
        {
            CurrentState.OnUpdate();
        }
    }

    void SetNext_State(State newState)
    {
        CurrentState.OnExit();
        CurrentState = newState;
        CurrentState.OnEnter();
    }
}
