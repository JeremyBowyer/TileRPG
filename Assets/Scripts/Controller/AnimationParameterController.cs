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
    }

    void ClearBools()
    {
        foreach (string _bool in _bools)
        {
            _animator.SetBool(_bool, false);
        }
    }

    public void SetBool(string _bool)
    {
        ClearBools();
        _animator.SetBool(_bool, true);
    }

    public void SetTrigger(string _trigger)
    {
        _animator.SetTrigger(_trigger);
    }

    public void SetTrigger(string _trigger, UnityAction _callback)
    {
        _animator.SetTrigger(_trigger);
        StartCoroutine(OnCompleteAnimation(_trigger, _callback));
    }

    IEnumerator OnCompleteAnimation(string param, UnityAction _callback)
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
