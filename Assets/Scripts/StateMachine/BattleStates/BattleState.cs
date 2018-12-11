using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class BattleState : State
{
    public CameraController cameraRig { get { return gc.cameraRig; } }
    public BattleUIController battleUiController { get { return gc.battleUiController; } }
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
        base.AddListeners();
    }

    protected override void RemoveListeners()
    {
        base.RemoveListeners();
    }

    protected override void OnClick(object sender, InfoEventArgs<RaycastHit> e)
    {
    }

    protected override void OnHoverEnter(object sender, InfoEventArgs<GameObject> e)
    {

    }

    protected override void OnHoverExit(object sender, InfoEventArgs<GameObject> e)
    {

    }

    protected override void OnMove(object sender, InfoEventArgs<Point> e)
    {
    }

    protected override void OnFire(object sender, InfoEventArgs<int> e)
    {
    }

    protected override void OnCancel(object sender, InfoEventArgs<int> e)
    {
    }

}