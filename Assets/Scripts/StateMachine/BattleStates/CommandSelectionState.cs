using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class CommandSelectionState : BaseAbilityMenuState
{
    protected override void LoadMenu()
    {
        if (menuOptions == null)
        {
            menuTitle = "Commands";
            menuOptions = new Dictionary<string, UnityAction>(3);
            menuOptions.Add("Move", Move);
            menuOptions.Add("Attack", Attack);
            menuOptions.Add("End Turn", EndTurn);
        }
        abilityMenuPanelController.Show(menuTitle, menuOptions);
    }

    protected override void Confirm()
    {

    }

    protected override void Cancel()
    {
        owner.ChangeState<ExploreState>();
    }

    protected void Move()
    {
        owner.ChangeState<MoveTargetState>();
    }

    protected void Attack()
    {
        Debug.Log("Attack");
        AttackTargetState.attackAbility = new AttackAbility();
        owner.ChangeState<AttackTargetState>();
    }

    protected void EndTurn()
    {
        Debug.Log("End Turn");
        owner.ChangeState<SelectUnitState>();
    }

}