using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DishOutDamageState : BattleState
{
    IEnumerator delayCoroutine;

    public override bool IsInterruptible
    {
        get { return false; }
    }
    public override bool IsMaster
    {
        get { return false; }
    }

    public override List<Type> AllowedTransitions
    {
        get
        {
            return new List<Type>
            {
            typeof(UnitTurnState),
            typeof(DishOutDamageState)
            };
        }
        set { }
    }

    public override void Enter()
    {
        InTransition = true;
        if (bc.damageQueue.Count == 0)
        {
            InTransition = false;
            bc.ChangeState<UnitTurnState>();
            return;
        }
        KeyValuePair<CharController, Damage[]> damagePackage = bc.damageQueue.Dequeue();

        if (damagePackage.Key.IsDead || !damagePackage.Key.gameObject.activeInHierarchy)
        {
            InTransition = false;
            bc.ChangeState<DishOutDamageState>();
        } else
        {
            bc.FollowTarget(damagePackage.Key.transform);
            damagePackage.Key.TakeDamage(damagePackage.Value);
            StartCoroutine(CustomUtils.DelayAction(1.5f,AfterDamage));
        }
    }

    public void AfterDamage()
    {
        InTransition = false;
        bc.ChangeState<DishOutDamageState>();
    }

}
