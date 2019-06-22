using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class BattleState : State
{
    public BattleController bc;
    public CameraController cameraRig { get { return bc.cameraRig; } }
    public BattleUIController battleUiController { get { return bc.battleUiController; } }
    public SuperUIController superUiController { get { return bc.superUiController; } }
    public Grid grid { get { return bc.grid; } }
    public Pathfinding pathfinder { get { return bc.pathfinder; } }
    public AbilityMenuPanelController abilityMenuPanelController { get { return bc.abilityMenuPanelController; } }
    public List<GameObject> characters { get { return bc.characters; } }

    // Methods
    protected virtual void Awake()
    {
        bc = GameObject.Find("BattleController").GetComponent<BattleController>();
    }

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

    protected override void OnMouseMove(object sender, InfoEventArgs<Vector3> e)
    {
    }

    protected override void OnFire(object sender, InfoEventArgs<int> e)
    {
    }

    protected override void OnCancel(object sender, InfoEventArgs<int> e)
    {
    }

}