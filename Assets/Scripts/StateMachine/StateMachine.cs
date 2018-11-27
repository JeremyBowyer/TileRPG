using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class StateMachine : MonoBehaviour
{
    public Text debugText;
    public bool dequeueRunning;

    private Queue<QueuedTransition> stateQueue = new Queue<QueuedTransition>();
    public struct QueuedTransition
    {
        public State state;
        public StateArgs args;
    }

    protected State _currentState;
    public virtual State CurrentState
    {
        get { return _currentState; }
        set { }
    }
    public bool _inTransition
    {
        get { return _currentState.inTransition; }
        set { }
    }

    public bool _isInterruptable
    {
        get { return _currentState.isInterruptable; }
        set { }
    }

    // Methods
    public void Update()
    {
        if(!_currentState.inTransition && stateQueue.Count > 0)
        {
            _currentState.Exit();

            QueuedTransition queuedTransition = stateQueue.Dequeue();
            _currentState = queuedTransition.state;
            _currentState.args = queuedTransition.args;
            Debug.Log("Dequeuing State: " + _currentState.GetType().Name);
            _currentState.Enter();

            debugText.text = _currentState.GetType().Name;
        }
    }
    public virtual T GetState<T>() where T : State
    {
        T target = GetComponent<T>();
        if (target == null)
            target = gameObject.AddComponent<T>();
        return target;
    }

    public virtual void ChangeState<T>() where T : State
    {
        Transition(GetState<T>(), new StateArgs());
    }

    public virtual void ChangeState<T>(StateArgs args) where T : State
    {
        Transition(GetState<T>(), args);
    }

    protected virtual void EnqueueTransition(State value, StateArgs args)
    {
        QueuedTransition queuedTransition = new QueuedTransition()
        {
            state = value,
            args = args
        };
        stateQueue.Enqueue(queuedTransition);
        Debug.Log(value + " queued.");
    }

    protected virtual void Transition(State value, StateArgs args)
    {
        // If transitioning to same state, do nothing
        if (_currentState == value)
        {
            Debug.Log("Already in state: " + _currentState);
            return;
        }

        // If no state is active, assign new state and call Enter() and return
        if (_currentState == null)
        {
            _currentState = value;
            _currentState.args = args;
            _currentState.Enter();
            debugText.text = value.GetType().Name;
            return;
        }

        // If queue is not empty, add to queue. No jumping the line.
        // or current state is in transition and can't be interrupted, add to queue
        if (stateQueue.Count > 0 || (_inTransition && !_isInterruptable))
        {
            EnqueueTransition(value, args);
            return;
        }

        // If current state is in transition, interrupt if allowed.
        if (_inTransition)
        {
            _currentState.InterruptTransition();
            _currentState.Exit();
        }
        else
        {
            _currentState.Exit();
        }

        _currentState = value;
        _currentState.args = args;
        _currentState.Enter();
        debugText.text = value.GetType().Name;
    }
}