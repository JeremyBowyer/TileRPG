using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public abstract class BaseAbilityMenuState : BattleState
{
    protected string menuTitle;
    protected Dictionary<string, UnityAction> menuOptions;

    public override void Enter()
    {
        base.Enter();
        LoadMenu();
    }
    public override void Exit()
    {
        base.Exit();
        abilityMenuPanelController.Hide();
    }

    protected override void OnClick(object sender, InfoEventArgs<GameObject> e)
    {

    }

    protected override void OnFire(object sender, InfoEventArgs<int> e)
    {
        if (e.info == 0)
            Confirm();
        else
            OnCancel(sender, e);
    }
    protected override void OnMove(object sender, InfoEventArgs<Point> e)
    {
        if (e.info.x > 0 || e.info.y < 0)
            abilityMenuPanelController.Next();
        else
            abilityMenuPanelController.Previous();
    }
    protected abstract void LoadMenu();
    protected abstract void Confirm();
}