using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class StateMachine : MonoBehaviour
{
    public bool dequeueRunning;
    public List<State> waitingList;

    public Text currentStateText;

    private List<QueuedTransition> stateQueue = new List<QueuedTransition>();
    public struct QueuedTransition
    {
        public State state;
        public StateArgs args;
    }

    protected State _currentState;

    public virtual State CurrentState
    {
        get { return _currentState; }
        set
        {
            _currentState = value;
            if (currentStateText != null && _currentState != null)
            {
                //Debug.Log("Entering State: " + _currentState.GetType().Name);
                currentStateText.text = _currentState.GetType().Name;
            }
        }
    }
    public bool _inTransition
    {
        get
        {
            if (_currentState == null)
                return false;
            else
                return _currentState.inTransition;
        }
        set { }
    }

    public bool _isInterruptable
    {
        get
        {
            if (_currentState == null)
                return false;
            else
                return _currentState.isInterruptable;
        }
        set { }
    }

    public bool _isInterrupting
    {
        get
        {
            if (_currentState == null)
                return false;
            else
                return _currentState.isInterrupting;
        }
        set { }
    }

    public bool _isMaster
    {
        get
        {
            bool currentMaster = _currentState == null ? false : _currentState.isMaster;
            return (currentMaster);
        }
        set { }
    }

    public bool _queuedMaster
    {
        get
        {
            bool queuedMaster = false;
            foreach (QueuedTransition qt in stateQueue)
            {
                if (qt.state.isMaster)
                {
                    queuedMaster = true;
                    break;
                }
            }
            return (queuedMaster);
        }
        set { }
    }

    public bool _waitForStates
    {
        get
        {
            return waitingList.Count > 0;
        }
    }

    // Methods
    public void Update()
    {
        if ((CurrentState == null && stateQueue.Count == 0) || _waitForStates)
            return;

        if(!_inTransition && stateQueue.Count > 0)
        {
            QueuedTransition qt = stateQueue[0];
            stateQueue.RemoveAt(0);
            Debug.Log("Dequeuing State: " + qt.state.GetType().Name);
            Transition(qt.state, qt.args);
        }
    }
    public virtual T GetState<T>() where T : State
    {
        T target = GetComponent<T>();
        if (target == null || target.isBeingDestroyed)
            target = gameObject.AddComponent<T>();
        return target;
    }

    public void AddToWaitingList(State state)
    {
        if(!waitingList.Contains(state))
            waitingList.Add(state);
    }

    public void RemoveFromWaitingList(State state)
    {
        if(waitingList.Contains(state))
            waitingList.Remove(state);
    }

    public void ClearWaitingList()
    {
        waitingList.Clear();
    }

    public bool IsInWaitingList(State state)
    {
        return waitingList.Contains(state);
    }

    public virtual bool IsInQueue(Type stateType)
    {
        if (stateQueue.Count == 0)
            return false;

        foreach(QueuedTransition qt in stateQueue)
        {
            if (qt.state.GetType() == stateType)
            {
                return true;
            }
        }

        return false;
    }

    public virtual State ChangeState<T>() where T : State
    {
        State state = ChangeState<T>(new StateArgs());
        return state;
    }

    public virtual State ChangeState<T>(StateArgs args) where T : State
    {
        Type targetState = typeof(T);
        State state = GetState<T>();
        // Check if transition is even valid
        if (!ValidateTransition(targetState))
            return state;

        // If queue is not empty, add to queue. No jumping the line.
        // or current state is in transition and can't be interrupted, add to queue
        // or is currently being interuppted, in which case do not trigger new interruption, add to queue instead
        if (stateQueue.Count > 0 || (_inTransition && !_isInterruptable) || _isInterrupting)
        {
            EnqueueTransition(GetState<T>(), args);
            return state;
        }

        // If FSM is waiting on other states to finish before proceeding, current state can be exited.
        if (_waitForStates && !_inTransition)
        {
            EnqueueTransition(GetState<T>(), args);
            CurrentState.Exit();
            CurrentState = null;
            return state;
        }

        Transition(state, args);
        return state;
    }

    protected virtual bool ValidateTransition(Type targetType)
    {
        if (CurrentState == null)
            return true;

        // Add universally allowed transitions
        List<Type> allowedTransitions = CurrentState.AllowedTransitions;
        allowedTransitions.Add(typeof(IdleState));
        if (CurrentState is BattleState)
        {
            allowedTransitions.Add(typeof(VictorySequence));
        }

        /* ------------------ */
        /* INVALID Conditions */
        /* ------------------ */
        /*
        // If transitioning to same state, do nothing
        if (CurrentState.GetType() == targetType)
        {
            Debug.Log("Already in state: " + CurrentState.GetType());
            return false;
        }
        */
        // If current state is master and in transition, or a master is queued, don't allow transition
        if ((_isMaster && _inTransition) || _queuedMaster)
        {
            Debug.Log(CurrentState.GetType().Name + " is a master state or a master state is queued, no transitions allowed.");
            return false;
        }

        // If current state doesn't allow transition to new state, do nothing.
        // Idle state can transition to anything
        if (!allowedTransitions.Contains(targetType) && stateQueue.Count == 0 && CurrentState.GetType() != typeof(IdleState))
        {
            Debug.Log(CurrentState.GetType().Name + " cannot transition to " + targetType.Name);
            return false;
        }

        /* ---------------- */
        /* VALID Conditions */
        /* ---------------- */

        // All battle states should be able to transition to victory sequence
        // and should also clear queue.
        if (CurrentState is BattleState && targetType == typeof(VictorySequence))
        {
            //Debug.Log("Clearing Queue to make way for: " + targetType.Name);
            //ClearQueue();
            return true;
        }

        // All states should be able to transition into WaitForPermissionState
        // except master states
        if (!CurrentState.isMaster && targetType == typeof(WaitForPermissionState))
        {
            return true;
        }

        return true;
    }

    protected virtual void ClearQueue()
    {
        stateQueue.Clear();
    }

    protected virtual void EnqueueTransition(State value, StateArgs args)
    {
        QueuedTransition queuedTransition = new QueuedTransition()
        {
            state = value,
            args = args
        };
        stateQueue.Add(queuedTransition);
        if(CurrentState != null)
            Debug.Log("Current State: " + CurrentState.GetType().Name + " Queued State: " + value.GetType().Name);
        else
            Debug.Log("Current State: NULL Queued State: " + value.GetType().Name);
    }

    protected virtual void Transition(State value, StateArgs args)
    {
        // If no state is active, assign new state and call Enter() and return
        if (CurrentState == null)
        {
            CurrentState = value;
            CurrentState.args = args;
            CurrentState.Enter();
            return;
        }

        // If current state is in transition, interrupt if allowed.
        if (_inTransition && _isInterruptable)
        {
            CurrentState.InterruptTransition();
            Debug.Log("Interrupting state: " + CurrentState.GetType().Name);
            CurrentState.Exit();
        }
        else
        {
            CurrentState.Exit();
        }
        CurrentState = value;
        CurrentState.args = args;
        CurrentState.Enter();
    }
}