using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class BattleState : State
{
    protected BattleController owner;
    public CameraController cameraRig { get { return owner.cameraRig; } }
    public Grid grid { get { return owner.grid; } }
    public Pathfinding pathfinder { get { return owner.pathfinder; } }
    public Node node { get { return owner.node; } set { owner.node = value; } }
    public AbilityMenuPanelController abilityMenuPanelController { get { return owner.abilityMenuPanelController; } }
    public Turn turn { get { return owner.turn; } }
    public List<Player> characters { get { return owner.characters; } }

    protected virtual void Awake()
    {
        owner = GetComponent<BattleController>();
    }

    protected override void AddListeners()
    {
        UserInputController.clickEvent += OnClick;
    }

    protected override void RemoveListeners()
    {
        UserInputController.clickEvent -= OnClick;
    }

    protected virtual void OnClick(object sender, InfoEventArgs<GameObject> e)
    {
        Debug.Log("Battle State Click");
    }

    protected virtual void OnMove(object sender, InfoEventArgs<Point> e)
    {
        Debug.Log("Battle State Move");
    }

    protected virtual void OnFire(object sender, InfoEventArgs<int> e)
    {
        Debug.Log("Battle State Fire");
    }

}