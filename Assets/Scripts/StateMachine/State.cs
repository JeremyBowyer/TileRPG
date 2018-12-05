using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class State : MonoBehaviour
{
    // Fields
    private bool _inTransition;
    public bool inTransition
    {
        get { return _inTransition; }
        set
        {
            _inTransition = value;
            if (args.waitingStateMachines == null)
                return;
            foreach(StateMachine sm in args.waitingStateMachines)
            {
                if (_inTransition)
                {
                    if(!sm.IsInWaitingList(this))
                        sm.AddToWaitingList(this);
                }
                else
                {
                    while(sm.IsInWaitingList(this))
                        sm.RemoveFromWaitingList(this);
                }
            }
        }
    }
    public bool isInterrupting = false;
    public bool isBeingDestroyed = false;
    public StateArgs args;


    // References
    protected GameController gc;

    // Properties
    public virtual bool isInterruptable
    {
        get { return false; }
    }
    public virtual bool isMaster
    {
        get { return false; }
    }

    public virtual List<Type> AllowedTransitions
    {
        get { return new List<Type>(); }
        set { }
    }


    // Methods
    protected virtual void Awake()
    {
        gc = GameObject.Find("GameController").GetComponent<GameController>();
    }

    public virtual void Enter()
    {
        AddListeners();
    }

    public virtual void Exit()
    {
        RemoveListeners();
        isBeingDestroyed = true;
        Destroy(this);
    }

    public virtual void InterruptTransition()
    {
    }

    public virtual void Permit(bool _permitted)
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