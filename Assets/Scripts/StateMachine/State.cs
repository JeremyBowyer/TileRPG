using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class State : MonoBehaviour
{
    // References
    public InputEventHandler events;

    // Fields
    private bool _inTransition;
    public bool InTransition
    {
        get { return _inTransition; }
        set
        {
            _inTransition = value;
            // If there are no waiting SM, return
            if (args.waitingStateMachines == null)
                return;

            // If there are SM waiting...
            foreach (StateMachine sm in args.waitingStateMachines)
            {
                // ...and this state is being entered into, add this state to their waiting list
                if (_inTransition)
                {
                    if(!sm.IsInWaitingList(this))
                        sm.AddToWaitingList(this);
                }
                // ...and this state is being exited, remove from their waiting list
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

    // Properties
    public virtual bool IsInterruptible
    {
        get { return false; }
    }
    public virtual bool IsMaster
    {
        get { return false; }
    }

    public virtual List<Type> AllowedTransitions
    {
        get { return new List<Type>(); }
        set { }
    }

    protected virtual void Awake()
    {
        events = GameObject.FindGameObjectWithTag("InputEventHandler").GetComponent<InputEventHandler>();
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

    protected virtual void OnDestroy()
    {
        RemoveListeners();
    }

    protected virtual void AddListeners()
    {
        UserInputController.clickEvent += OnClick;
        UserInputController.cancelEvent += OnCancel;
        UserInputController.hoverEnterEvent += OnHoverEnter;
        UserInputController.hoverExitEvent += OnHoverExit;
        UserInputController.moveEvent += OnMove;
        UserInputController.moveMouseEvent += OnMouseMove;
        UserInputController.keyDownEvent += OnKeyDown;
    }

    protected virtual void RemoveListeners()
    {
        UserInputController.clickEvent -= OnClick;
        UserInputController.cancelEvent -= OnCancel;
        UserInputController.hoverEnterEvent -= OnHoverEnter;
        UserInputController.hoverExitEvent -= OnHoverExit;
        UserInputController.moveEvent -= OnMove;
        UserInputController.moveMouseEvent -= OnMouseMove;
        UserInputController.keyDownEvent -= OnKeyDown;
    }

    protected virtual void OnClick(object sender, InfoEventArgs<RaycastHit> e)
    {
    }

    protected virtual void OnHoverEnter(object sender, InfoEventArgs<GameObject> e)
    {
    }

    protected virtual void OnHoverExit(object sender, InfoEventArgs<GameObject> e)
    {
    }

    protected virtual void OnMove(object sender, InfoEventArgs<Point> e)
    {
    }

    protected virtual void OnMouseMove(object sender, InfoEventArgs<Vector3> e)
    {
    }

    protected virtual void OnCancel(object sender, InfoEventArgs<int> e)
    {
    }

    protected virtual void OnKeyDown(object sender, InfoEventArgs<KeyCode> e)
    {
    }

}