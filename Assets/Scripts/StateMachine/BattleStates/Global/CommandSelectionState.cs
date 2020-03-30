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
            typeof(ItemSelectionState),
            typeof(SelectUnitState)
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
            InTransition = false;
        }

    }

    protected override void LoadMenu()
    {
        abilityMenuPanelController.Show("Commands");
        abilityMenuPanelController.AddEntry(new KeyValuePair<string, UnityAction>(bc.CurrentCharacter.MovementAbility.mName, Move), bc.CurrentCharacter.Stats.curAP > 0);
        abilityMenuPanelController.AddEntry(new KeyValuePair<string, UnityAction>(bc.CurrentCharacter.AttackAbility.AbilityName, Attack), bc.CurrentCharacter.AttackAbility.ValidateCost(bc.CurrentCharacter), bc.CurrentCharacter.AttackAbility);
        if (bc.CurrentCharacter.Spells != null && bc.CurrentCharacter.Spells.Count > 0)
            abilityMenuPanelController.AddEntry(new KeyValuePair<string, UnityAction>("Spells", Spells), true);
        if(bc.CurrentCharacter.HasPersonalPassive(typeof(BagAccess)))
            abilityMenuPanelController.AddEntry(new KeyValuePair<string, UnityAction>("Use Item", Items), true);
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
        GlobalAudioController.instance.Play("default_confirm_click");
        bc.ChangeState<MoveTargetState>();
    }

    protected void Attack()
    {
        GlobalAudioController.instance.Play("default_confirm_click");
        StateArgs attackTargetArgs = new StateArgs
        {
            attackAbility = bc.CurrentCharacter.AttackAbility
        };
        bc.ChangeState<AttackTargetState>(attackTargetArgs);
    }

    protected void Spells()
    {
        GlobalAudioController.instance.Play("default_confirm_click");
        bc.ChangeState<SpellSelectionState>();
    }

    protected void Items()
    {
        GlobalAudioController.instance.Play("default_confirm_click");
        BattleController.instance.ChangeState<ItemSelectionState>();
    }

    protected void EndTurn()
    {
        GlobalAudioController.instance.Play("default_cancel_click");
        bc.ChangeState<SelectUnitState>();
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