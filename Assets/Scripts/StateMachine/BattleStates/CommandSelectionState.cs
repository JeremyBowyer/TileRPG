using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class CommandSelectionState : BaseAbilityMenuState
{
    
    protected override void LoadMenu()
    {
        menuOptions = new Dictionary<string, UnityAction>();
        menuTitle = "Commands";
        menuOptions.Add("Move", Move);
        menuOptions.Add(gc.currentCharacter.attackAbility.AbilityName, Attack);
        menuOptions.Add("Spells", Spells);
        menuOptions.Add("End Turn", EndTurn);
        abilityMenuPanelController.Show(menuTitle, menuOptions);
    }

    protected override void Confirm()
    {

    }

    protected override void OnCancel(object sender, InfoEventArgs<int> e)
    {
        gc.ChangeState<ExploreState>();
    }

    protected void Move()
    {
        gc.ChangeState<MoveTargetState>();
    }

    protected void Attack()
    {
        AttackTargetState.attackAbility = gc.currentCharacter.attackAbility;
        gc.ChangeState<AttackTargetState>();
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