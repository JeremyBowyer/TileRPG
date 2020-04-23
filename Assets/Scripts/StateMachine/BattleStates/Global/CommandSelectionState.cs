using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CommandSelectionState : BattleState
{
    
    public override List<Type> AllowedTransitions
    {
        get
        {
            return new List<Type>
            {
            typeof(ExploreBattleState),
            typeof(MoveTargetState),
            typeof(AttackTargetState),
            typeof(ItemSelectionState),
            typeof(SelectUnitState),
            typeof(SpellCharacterTargetState),
            typeof(SpellEnvironmentSplashTargetState),
            typeof(SpellEmanationCharacterTargetState),
            typeof(SpellEnvironmentPathTargetState),
            typeof(UnitTurnState)
            };
        }
        set { }
    }

    public override void Enter()
    {
        InTransition = true;
        if (bc.CurrentCharacter == null || bc.CurrentCharacter.IsDead)
        {
            InTransition = false;
            bc.ChangeState<SelectUnitState>();
            return;
        }
        else
        {
            base.Enter();
            bc.FollowTarget(bc.CurrentCharacter.transform);
            BattleUIController.instance.LoadCharacter(null, bc.CurrentCharacter);
            BattleUIController.instance.ShowTrayAndStatus(true, true);
            InTransition = false;
        }

    }

    public override void Exit()
    {
        base.Exit();
        BattleUIController.instance.ShowTrayAndStatus(false, true);
    }

    protected override void OnCancel(object sender, InfoEventArgs<int> e)
    {
        BattleUIController.instance.ExploreBattle();
    }

    protected override void OnHoverEnter(object sender, InfoEventArgs<GameObject> e)
    {
        OutlineTargetCharacter(sender, e);
    }

    protected override void OnHoverExit(object sender, InfoEventArgs<GameObject> e)
    {
        RemoveOutlineTargetCharacter();
    }

}
