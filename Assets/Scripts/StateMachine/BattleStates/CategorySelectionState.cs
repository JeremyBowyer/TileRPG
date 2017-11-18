using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class CategorySelectionState : BaseAbilityMenuState
{
    protected override void LoadMenu()
    {
        if (menuOptions == null)
        {
            menuTitle = "Action";
            menuOptions = new Dictionary<string, UnityAction>(3);
            menuOptions.Add("Attack", delegate { });
            menuOptions.Add("White Magic", delegate { });
            menuOptions.Add("Black Magic", delegate { });
        }

        abilityMenuPanelController.Show(menuTitle, menuOptions);
    }
    protected override void Confirm()
    {
        switch (abilityMenuPanelController.selection)
        {
            case 0:
                AttackTargetState.attackAbility = new AttackAbility();
                owner.ChangeState<AttackTargetState>();
                break;
            case 1:
                SetCategory(0);
                break;
            case 2:
                SetCategory(1);
                break;
        }
    }

    protected override void Cancel()
    {
        owner.ChangeState<CommandSelectionState>();
    }

    void SetCategory(int index)
    {
        ActionSelectionState.category = index;
        owner.ChangeState<ActionSelectionState>();
    }
}