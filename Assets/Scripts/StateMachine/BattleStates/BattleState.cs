using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class BattleState : State
{
    public BattleController bc { get { return BattleController.instance; } }
    public CameraController cameraRig { get { return bc.cameraRig; } }
    public BattleUIController battleUI { get { return bc.battleUI; } }
    public SuperUIController superUI { get { return bc.lc.superUI; } }
    public Grid grid { get { return bc.grid; } }
    public Pathfinding pathfinder { get { return bc.pathfinder; } }
    public List<GameObject> characters { get { return bc.characters; } }

    // Methods
    protected override void Awake()
    {
        base.Awake();
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

    protected override void OnCancel(object sender, InfoEventArgs<int> e)
    {
    }

    protected void OutlineTargetCharacter(object sender, InfoEventArgs<GameObject> e)
    {
        CharController target = e.info.gameObject.GetComponent<CharController>();

        if (target != null)
        {
            events.LoadTargetCharacter(target);
            Color clr = target is EnemyController ? CustomColors.Hostile : CustomColors.Heal;
            if (target != bc.CurrentCharacter)
                events.OutlineCharacter(target, clr, _mode: Outline.Mode.OutlineAndSilhouette, _width: 2f);
        }
    }

    protected void RemoveOutlineTargetCharacter()
    {
        events.UnloadTargetCharacter();
        events.RemoveOutlines();
    }

}