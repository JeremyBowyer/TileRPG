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

    protected override void LoadMenu()
    {
        abilityMenuPanelController.Show("Commands");
        abilityMenuPanelController.AddEntry(new KeyValuePair<string, UnityAction>(gc.currentCharacter.MovementAbility.mName, Move), gc.currentCharacter.Stats.curAP > 0);
        abilityMenuPanelController.AddEntry(new KeyValuePair<string, UnityAction>(gc.currentCharacter.AttackAbility.AbilityName, Attack), gc.currentCharacter.AttackAbility.ValidateCost(gc.currentCharacter));
        if (gc.currentCharacter.Spells != null && gc.currentCharacter.Spells.Count > 0)
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
        gc.ChangeState<ExploreBattleState>();
    }

    protected void Move()
    {
        gc.ChangeState<MoveTargetState>();
    }

    protected void Attack()
    {
        StateArgs attackTargetArgs = new StateArgs
        {
            attackAbility = gc.currentCharacter.AttackAbility
        };
        gc.ChangeState<AttackTargetState>(attackTargetArgs);
    }

    protected void Spells()
    {
        gc.ChangeState<SpellSelectionState>();
    }

    protected void EndTurn()
    {
        gc.ChangeState<SelectUnitState>();
    }

}