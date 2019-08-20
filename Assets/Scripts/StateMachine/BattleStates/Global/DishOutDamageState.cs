using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DishOutDamageState : BattleState
{

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
            typeof(CheckForTurnEndState)
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
            bc.ChangeState<CheckForTurnEndState>();
            return;
        }

        KeyValuePair<CharController, Damage[]> damagePackage = bc.damageQueue.Dequeue();
        damagePackage.Key.Damage(damagePackage.Value);

        InTransition = false;
        bc.ChangeState<DishOutDamageState>();

    }

}
