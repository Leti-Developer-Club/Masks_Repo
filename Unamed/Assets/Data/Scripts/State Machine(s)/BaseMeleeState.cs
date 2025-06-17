using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMeleeState : State
{
    public float Duration;//how long state should lasy before moving on
    protected Animator animator;
    protected bool shouldCombo;
    protected int attackIndex;

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
