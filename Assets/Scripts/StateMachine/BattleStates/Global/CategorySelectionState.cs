using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

public class CategorySelectionState : BaseAbilityMenuState
{

    public override List<Type> AllowedTransitions
    {
        get
        {
            return new List<Type>
            {
            typeof(AttackTargetState),
            typeof(ActionSelectionState),
            typeof(CommandSelectionState)
            };
        }
        set { }
    }

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
                StateArgs attackTargetArgs = new StateArgs
                {
                    attackAbility = gc.currentCharacter.attackAbility
                };
                gc.ChangeState<AttackTargetState>(attackTargetArgs);
                break;
            case 1:
                SetCategory(0);
                break;
            case 2:
                SetCategory(1);
                break;
        }
    }

    protected override void OnCancel(object sender, InfoEventArgs<int> e)
    {
        gc.ChangeState<CommandSelectionState>();
    }

    void SetCategory(int index)
    {
        ActionSelectionState.category = index;
        gc.ChangeState<ActionSelectionState>();
    }
}