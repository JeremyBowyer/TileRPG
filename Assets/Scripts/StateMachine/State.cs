using UnityEngine;
using System.Collections;
public abstract class State : MonoBehaviour
{
    public bool inTransition;
    public bool isInterruptable = false;
    public StateArgs args;

    public virtual void Enter()
    {
        AddListeners();
    }

    public virtual void Exit()
    {
        RemoveListeners();
    }

    public virtual void InterruptTransition()
    {
    }

    protected virtual void OnDestroy()
    {
        RemoveListeners();
    }

    protected virtual void AddListeners()
    {
    }
    protected virtual void RemoveListeners()
    {
    }


}