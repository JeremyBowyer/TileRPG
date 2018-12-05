using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class BattleState : State
{
    public CameraController cameraRig { get { return gc.cameraRig; } }
    public BattleUIController uiController { get { return gc.uiController; } }
    public Grid grid { get { return gc.grid; } }
    public Pathfinding pathfinder { get { return gc.pathfinder; } }
    public AbilityMenuPanelController abilityMenuPanelController { get { return gc.abilityMenuPanelController; } }
    public List<GameObject> characters { get { return gc.characters; } }

    public override void Enter()
    {
        base.Enter();
    }

    protected override void AddListeners()
    {
        UserInputController.clickEvent += OnClick;
        UserInputController.cancelEvent += OnCancel;
        UserInputController.hoverEnterEvent += OnHoverEnter;
        UserInputController.hoverExitEvent += OnHoverExit;
    }

    protected override void RemoveListeners()
    {
        UserInputController.clickEvent -= OnClick;
        UserInputController.cancelEvent -= OnCancel;
        UserInputController.hoverEnterEvent -= OnHoverEnter;
        UserInputController.hoverExitEvent -= OnHoverExit;
    }

    protected virtual void OnClick(object sender, InfoEventArgs<GameObject> e)
    {
        Debug.Log("Battle State Click");
    }

    protected virtual void OnHoverEnter(object sender, InfoEventArgs<GameObject> e)
    {

    }

    protected virtual void OnHoverExit(object sender, InfoEventArgs<GameObject> e)
    {

    }

    protected virtual void OnMove(object sender, InfoEventArgs<Point> e)
    {
        Debug.Log("Battle State Move");
    }

    protected virtual void OnFire(object sender, InfoEventArgs<int> e)
    {
        Debug.Log("Battle State Fire");
    }

    protected virtual void OnCancel(object sender, InfoEventArgs<int> e)
    {
        Debug.Log("Battle State Cancel");
    }

}