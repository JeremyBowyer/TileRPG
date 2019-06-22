using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

public class CommandSelectionState : BaseAbilityMenuState
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
            typeof(SpellSelectionState),
            typeof(SelectUnitState)
            };
        }
        set { }
    }

    public override void Enter()
    {
        inTransition = true;
        bc.cameraRig.isFollowing = true;
        base.Enter();
        inTransition = false;
    }

    protected override void LoadMenu()
    {
        abilityMenuPanelController.Show("Commands");
        abilityMenuPanelController.AddEntry(new KeyValuePair<string, UnityAction>(bc.CurrentCharacter.MovementAbility.mName, Move), bc.CurrentCharacter.Stats.curAP > 0);
        abilityMenuPanelController.AddEntry(new KeyValuePair<string, UnityAction>(bc.CurrentCharacter.AttackAbility.AbilityName, Attack), bc.CurrentCharacter.AttackAbility.ValidateCost(bc.CurrentCharacter));
        if (bc.CurrentCharacter.Spells != null && bc.CurrentCharacter.Spells.Count > 0)
            abilityMenuPanelController.AddEntry(new KeyValuePair<string, UnityAction>("Spells", Spells), true);
        abilityMenuPanelController.AddEntry(new KeyValuePair<string, UnityAction>("End Turn", EndTurn), true);

        /*
        menuOptions = new Dictionary<string, UnityAction>();
        menuTitle = "Commands";
        menuOptions.Add("Move", Move);
        menuOptions.Add(gc.currentCharacter.AttackAbility.AbilityName, Attack);
        if(gc.currentCharacter.Spells != null && gc.currentCharacter.Spells.Count > 0)
            menuOptions.Add("Spells", Spells);
        menuOptions.Add("End Turn", EndTurn);
        abilityMenuPanelController.Show(menuTitle, menuOptions);
        */
    }

    protected override void Confirm()
    {

    }

    protected override void OnCancel(object sender, InfoEventArgs<int> e)
    {
        bc.ChangeState<ExploreBattleState>();
    }

    protected void Move()
    {
        bc.ChangeState<MoveTargetState>();
    }

    protected void Attack()
    {
        StateArgs attackTargetArgs = new StateArgs
        {
            attackAbility = bc.CurrentCharacter.AttackAbility
        };
        bc.ChangeState<AttackTargetState>(attackTargetArgs);
    }

    protected void Spells()
    {
        bc.ChangeState<SpellSelectionState>();
    }

    protected void EndTurn()
    {
        bc.ChangeState<SelectUnitState>();
    }

}