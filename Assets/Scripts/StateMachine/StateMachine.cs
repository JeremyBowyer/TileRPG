using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StateMachine : MonoBehaviour
{
    public Text debugText;
    public virtual State CurrentState
    {
        get { return _currentState; }
        set { Transition(value); }
    }
    protected State _currentState;
    public bool _inTransition;

    public virtual T GetState<T>() where T : State
    {
        T target = GetComponent<T>();
        if (target == null)
            target = gameObject.AddComponent<T>();
        return target;
    }

    public virtual void ChangeState<T>() where T : State
    {
        CurrentState = GetState<T>();
    }

    protected virtual void Transition(State value)
    {
        if (_currentState == value || _inTransition)
        {
            if (_inTransition)
            {
                Debug.Log("State in transition! Current State: " + _currentState);
            }
            return;
        }
        _inTransition = true;

        if (_currentState != null)
            _currentState.Exit();

        _currentState = value;
        debugText.text = value.GetType().Name;

        if (_currentState != null)
            _currentState.Enter();

        _inTransition = false;
    }
}