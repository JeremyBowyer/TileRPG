using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationParameterController : MonoBehaviour {

    public List<string> _bools;
    public List<string> _triggers;

    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _animator.applyRootMotion = false; // Running animation would move character forward without this
    }

    public void Pause()
    {
        _animator.enabled = false;
    }

    public void Resume()
    {
        _animator.enabled = true;
    }

    void ClearBools()
    {
        foreach (AnimatorControllerParameter param in _animator.parameters)
        {
            if(param.type == AnimatorControllerParameterType.Bool)
                _animator.SetBool(param.name, false);
        }
    }

    public void SetBool(string _name)
    {
        ClearBools();
        _animator.SetBool(_name, true);
    }

    public void SetBool(string _name, bool _bool)
    {
        if(_bool)
            ClearBools();
        _animator.SetBool(_name, _bool);
    }

    public void SetFloat(string _float, float _value)
    {
        _animator.SetFloat(_float, _value);
    }

    public void SetTrigger(string _trigger)
    {
        _animator.SetTrigger(_trigger);
    }

    public void SetTrigger(string _trigger, Action _callback)
    {
        _animator.SetTrigger(_trigger);
        StartCoroutine(OnCompleteAnimation(_trigger, _callback));
    }

    IEnumerator OnCompleteAnimation(string param, Action _callback)
    {
        // **************************************************** //
        // PARAMETER STRING MUST BE IDENTICAL TO ANIMATION NAME //
        // **************************************************** //
        yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).IsName(param));
        while (_animator.GetCurrentAnimatorStateInfo(0).IsName(param) && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= .99f)
        {
            yield return null;
        }
        _callback();
    }
}
